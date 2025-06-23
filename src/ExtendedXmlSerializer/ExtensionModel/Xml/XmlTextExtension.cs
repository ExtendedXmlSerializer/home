using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ISerializers = ExtendedXmlSerializer.ContentModel.Content.ISerializers;
// ReSharper disable TooManyDependencies

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlTextExtension : ISerializerExtension
	{
		public static XmlTextExtension Default { get; } = new XmlTextExtension();

		XmlTextExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<IContentMember, ContentMember>()
			            .RegisterInstance<IContentMembers>(ContentMembers.Instance)
			            .Decorate<IMemberSerializations, MemberSerializations>()
			            .Decorate<IMemberSerializers, MemberSerializers>()
			            .Decorate<IInnerContentActivation, InnerContentActivation>();

		public void Execute(IServices parameter) {}

		sealed class MemberSerializations : IMemberSerializations
		{
			readonly ISpecifications       _specifications;
			readonly IMemberSerializations _serialization;
			readonly IContentMembers       _content;

			[UsedImplicitly]
			public MemberSerializations(IMemberSerializations serialization, IContentMembers members)
				: this(Specifications.Instance, serialization, members) {}

			public MemberSerializations(ISpecifications specifications, IMemberSerializations serialization,
			                            IContentMembers content)
			{
				_specifications = specifications;
				_serialization  = serialization;
				_content        = content;
			}

			public IMemberSerialization Get(TypeInfo parameter)
			{
				var serialization = _serialization.Get(parameter);
				var content       = _content.Get(serialization);
				var result = content != null
					             ? new MemberSerialization(_specifications.Get(content.Profile.MemberType),
					                                       serialization, content)
					             : serialization;
				return result;
			}
		}

		interface ISpecifications : IParameterizedSource<TypeInfo, ISpecification<string>> {}

		sealed class Specifications : ISpecifications
		{
			readonly static ISpecification<string> True  = AlwaysSpecification<string>.Default,
			                                       False = new DelegatedSpecification<string>(string.IsNullOrEmpty);

			public static Specifications Instance { get; } = new Specifications();

			Specifications() : this(IsCollectionTypeSpecification.Default, True, False) {}

			readonly ISpecification<TypeInfo> _specification;
			readonly ISpecification<string>   _true, _false;

			public Specifications(ISpecification<TypeInfo> specification, ISpecification<string> @true,
			                      ISpecification<string> @false)
			{
				_specification = specification;
				_true          = @true;
				_false         = @false;
			}

			public ISpecification<string> Get(TypeInfo parameter)
				=> _specification.IsSatisfiedBy(parameter) ? _true : _false;
		}

		sealed class MemberSerialization : IMemberSerialization
		{
			readonly ISpecification<string> _specification;
			readonly IMemberSerialization   _serialization;
			readonly IMemberSerializer      _content;

			public MemberSerialization(ISpecification<string> specification, IMemberSerialization serialization,
			                           IMemberSerializer content)
			{
				_specification = specification;
				_serialization = serialization;
				_content       = content;
			}

			public ImmutableArray<IMemberSerializer> Get() => _serialization.Get();

			public IMemberSerializer Get(string parameter)
				=> _serialization.Get(parameter) ??
				   (parameter != "xmlns" && _specification.IsSatisfiedBy(parameter) ? _content : null);

			public ImmutableArray<IMemberSerializer> Get(object parameter) => _serialization.Get(parameter);
		}

		sealed class MemberSerializers : IMemberSerializers
		{
			readonly IConverters                _converters;
			readonly IContentMember             _member;
			readonly IIdentities                _identities;
			readonly IMemberSerializers         _members;
			readonly ISerializers               _serializers;
			readonly ContentModel.ISerializer                _serializer;
			readonly ISpecification<MemberInfo> _content;

			[UsedImplicitly]
			public MemberSerializers(ContentModel.ISerializer serializer, IIdentities identities, ISerializers serializers,
			                         IMemberSerializers members, IConverters converters, IContentMember member)
				: this(IsDefinedSpecification<XmlTextAttribute>.Default, serializer, identities, serializers, members,
				       converters, member) {}

			public MemberSerializers(ISpecification<MemberInfo> content, ContentModel.ISerializer serializer, IIdentities identities,
			                         ISerializers serializers, IMemberSerializers members, IConverters converters,
			                         IContentMember member)
			{
				_content     = content;
				_serializer  = serializer;
				_identities  = identities;
				_serializers = serializers;
				_members     = members;
				_converters  = converters;
				_member      = member;
			}

			public IMemberSerializer Get(IMember parameter)
			{
				var serializer = _members.Get(parameter);
				var result = _content.IsSatisfiedBy(parameter.Metadata)
					             ? Create(serializer, parameter.Metadata.ReflectedType)
					             : serializer;
				return result;
			}

			IMemberSerializer Create(IMemberSerializer serializer, Type owner)
			{
				var item = serializer.Profile.MemberType == typeof(string)
                    ? null  // strings should not be serialized as a collection of char
                    : CollectionItemTypeLocator.Default.Get(serializer.Profile.MemberType);
				var itemType = _member.Get(owner)
				                      .Item1;
				var member = (IMemberSerializer)new MemberSerializer(serializer, _converters.Get(itemType));
				var result = item != null
					             ? new ListSerializer(_serializer, _serializers, serializer, member,
					                                  _identities.Get(owner), item)
					             : member;
				return result;
			}
		}

		interface ILists : IParameterizedSource<ArrayList, IList> {}

		sealed class GenericList<T> : ILists
		{
			[UsedImplicitly]
			public static GenericList<T> Instance { get; } = new GenericList<T>();

			GenericList() {}

			public IList Get(ArrayList parameter) => parameter.OfType<T>()
			                                                  .ToList();
		}

		sealed class ListSerializer : IMemberSerializer
		{
			readonly static Generic<ILists> Lists = new Generic<ILists>(typeof(GenericList<>));

			readonly ContentModel.ISerializer                            _serializer;
			readonly ISerializers                           _serializers;
			readonly IMemberSerializer                      _default;
			readonly IMemberSerializer                      _item;
			readonly IIdentity                              _identity;
			readonly IParameterizedSource<ArrayList, IList> _lists;
			readonly TypeInfo                               _type;

			public ListSerializer(ContentModel.ISerializer serializer, ISerializers serializers, IMemberSerializer @default,
			                      IMemberSerializer item, IIdentity identity, TypeInfo type)
				: this(serializer, serializers, @default, item, identity, Lists.Get(type)(), type) {}

			public ListSerializer(ContentModel.ISerializer serializer, ISerializers serializers, IMemberSerializer @default,
			                      IMemberSerializer item, IIdentity identity, ILists lists, TypeInfo type)
			{
				_serializer  = serializer;
				_serializers = serializers;
				_default     = @default;
				_item        = item;
				_identity    = identity;
				_lists       = lists;
				_type        = type;
			}

			public object Get(IFormatReader parameter)
			{
				var reader = parameter.Get()
				                      .To<System.Xml.XmlReader>();

				var items = new ArrayList();

				while (!IdentityComparer.Default.Equals(parameter, _identity))
				{
					var value = _serializer.Get(parameter);
					items.Add(value);
					reader.Read();
					reader.MoveToContent();
				}

				if (reader.HasValue)
				{
					items.Add(_item.Get(parameter));
				}

				reader.Read();

				var result = _lists.Get(items);
				return result;
			}

			public void Write(IFormatWriter writer, object instance)
			{
				var list = _default.Access.Get(instance)
				                   .To<IList>();

				for (var i = 0; i < list.Count; i++)
				{
					var item = list[i];
					if (item != null)
					{
						var info = item.GetType()
						               .GetTypeInfo();
						var serializer = info == _type ? _item : _serializers.Get(info);
						serializer.Write(writer, item);
					}
				}
			}

			public IMember Profile => _item.Profile;

			public IMemberAccess Access => _item.Access;
		}

		sealed class InnerContentActivation : IInnerContentActivation
		{
			readonly ISpecification<TypeInfo> _collection;
			readonly IInnerContentActivation  _activator;
			readonly IContentMember           _member;

			[UsedImplicitly]
			public InnerContentActivation(IInnerContentActivation activator, IContentMember member)
				: this(IsCollectionTypeSpecification.Default, activator, member) {}

			public InnerContentActivation(ISpecification<TypeInfo> collection, IInnerContentActivation activator,
			                              IContentMember member)
			{
				_collection = collection;
				_activator  = activator;
				_member     = member;
			}

			public IInnerContentActivator Get(TypeInfo parameter)
			{
				var content = _activator.Get(parameter);
				var typeInfo = _member.Get(parameter)
				                      ?.Item2.MemberType;
				var result = typeInfo != null
					             ? new InnerContentActivator(content, _collection.IsSatisfiedBy(typeInfo))
					             : content;
				return result;
			}
		}

		sealed class InnerContentActivator : IInnerContentActivator
		{
			readonly IInnerContentActivator _activator;
			readonly bool                   _apply;

			public InnerContentActivator(IInnerContentActivator activator, bool apply)
			{
				_activator = activator;
				_apply     = apply;
			}

			public IInnerContent Get(IFormatReader parameter)
			{
				var monitor = new ConditionMonitor();
				if (_apply)
				{
					monitor.Apply();
				}

				var result = new InnerContent(_activator.Get(parameter), monitor);
				return result;
			}
		}

		sealed class InnerContent : IInnerContent
		{
			readonly IInnerContent    _content;
			readonly ConditionMonitor _monitor;

			public InnerContent(IInnerContent content, ConditionMonitor monitor)
			{
				_content = content;
				_monitor = monitor;
			}

			public IFormatReader Get() => _content.Get();

			public bool MoveNext() => _content.MoveNext() || _monitor.Apply();

			public void Reset()
			{
				_content.Reset();
			}

			public object Current => _content.Current;
		}

		interface IContentMembers : IParameterizedSource<IMemberSerialization, IMemberSerializer> {}

		sealed class ContentMembers : IContentMembers
		{
			public static ContentMembers Instance { get; } = new ContentMembers();

			ContentMembers() : this(IsDefinedSpecification<XmlTextAttribute>.Default.IsSatisfiedBy) {}

			readonly Func<MemberInfo, bool> _specification;

			public ContentMembers(Func<MemberInfo, bool> specification) => _specification = specification;

			public IMemberSerializer Get(IMemberSerialization parameter)
			{
				var serializers = parameter.Get();
				for (var i = 0; i < serializers.Length; i++)
				{
					var serializer = serializers[i];
					if (_specification(serializer.Profile.Metadata))
					{
						return serializer;
					}
				}

				return null;
			}
		}

		interface IContentMember : IParameterizedSource<TypeInfo, Tuple<TypeInfo, IMember>> {}

		sealed class ContentMember : ReferenceCacheBase<TypeInfo, Tuple<TypeInfo, IMember>>, IContentMember
		{
			readonly static Func<MemberInfo, bool> Specification
				= IsDefinedSpecification<XmlTextAttribute>.Default.IsSatisfiedBy;

			readonly ISpecification<TypeInfo> _collection;
			readonly Func<MemberInfo, bool>   _specification;
			readonly ITypeMembers             _members;

			[UsedImplicitly]
			public ContentMember(ITypeMembers members)
				: this(IsCollectionTypeSpecification.Default, Specification, members) {}

			public ContentMember(ISpecification<TypeInfo> collection, Func<MemberInfo, bool> specification,
			                     ITypeMembers members)
			{
				_collection    = collection;
				_specification = specification;
				_members       = members;
			}

			protected override Tuple<TypeInfo, IMember> Create(TypeInfo parameter)
			{
				var members = _members.Get(parameter);
				for (var i = 0; i < members.Length; i++)
				{
					var member = members[i];
					if (_specification(member.Metadata))
					{
						var type = _collection.IsSatisfiedBy(member.MemberType)
							           ? member.Metadata.GetCustomAttribute<XmlTextAttribute>()
							                   ?.Type.GetTypeInfo() ?? member.MemberType
							           : member.MemberType;
						return Tuple.Create(type, member);
					}
				}

				return null;
			}
		}

		sealed class MemberSerializer : IMemberSerializer
		{
			readonly IMemberSerializer _serializer;
			readonly IConverter        _converter;

			public MemberSerializer(IMemberSerializer serializer, IConverter converter)
			{
				_serializer = serializer;
				_converter  = converter;
				_serializer = serializer;
			}

			public object Get(IFormatReader parameter) => _converter.Parse(parameter.Get()
			                                                                        .To<System.Xml.XmlReader>()
			                                                                        .Value);

			public void Write(IFormatWriter writer, object instance)
			{
				var value   = _serializer.Access.Get(instance);
				var content = _converter.Format(value);
				writer.Content(content);
			}

			public IMember Profile => _serializer.Profile;

			public IMemberAccess Access => _serializer.Access;
		}
	}
}