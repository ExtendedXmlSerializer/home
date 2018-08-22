// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using ISerializers = ExtendedXmlSerializer.ContentModel.Content.ISerializers;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlTextExtension : ISerializerExtension
	{
		public static XmlTextExtension Default { get; } = new XmlTextExtension();

		XmlTextExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<IContentType, ContentType>()
			            .RegisterInstance<IContentMembers>(ContentMembers.Instance)
			            .Decorate<IMemberSerializations, MemberSerializations>()
			            .Decorate<IMemberSerializers, MemberSerializers>()
			            .Decorate<IInnerContentActivation, InnerContentActivation>();

		public void Execute(IServices parameter) {}

		sealed class MemberSerializations : IMemberSerializations
		{
			readonly ISpecification<string> _true;
			readonly ISpecification<string> _false;
			readonly IMemberSerializations  _serialization;
			readonly IContentMembers        _members;

			public MemberSerializations(IMemberSerializations serialization, IContentMembers members)
				: this(AlwaysSpecification<string>.Default, new DelegatedSpecification<string>(string.IsNullOrEmpty),
				       serialization, members) {}

			public MemberSerializations(ISpecification<string> @true, ISpecification<string> @false,
			                            IMemberSerializations serialization, IContentMembers members)
			{
				_true          = @true;
				_false         = @false;
				_serialization = serialization;
				_members       = members;
			}

			public IMemberSerialization Get(TypeInfo parameter)
			{
				var serialization = _serialization.Get(parameter);
				var member        = _members.Get(serialization);
				var result        = member != null ? Create(serialization, member) : serialization;
				return result;
			}

			MemberSerialization Create(IMemberSerialization serialization, IMemberSerializer member)
			{
				var specification = IsCollectionTypeSpecification.Default.IsSatisfiedBy(member.Profile.MemberType)
					                    ? _true
					                    : _false;
				var result = new MemberSerialization(specification, serialization, member);
				return result;
			}
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
			readonly ISpecification<MemberInfo> _content;
			readonly ISerializer                _serializer;
			readonly IIdentities                _identities;
			readonly ISerializers               _serializers;
			readonly IMemberSerializers         _members;
			readonly IConverters                _converters;
			readonly IContentType               _type;

			public MemberSerializers(ISerializer serializer, IIdentities identities, ISerializers serializers,
			                         IMemberSerializers members, IConverters converters, IContentType type)
				: this(IsContent.Instance, serializer, identities, serializers, members, converters, type) {}

			public MemberSerializers(ISpecification<MemberInfo> content, ISerializer serializer, IIdentities identities,
			                         ISerializers serializers, IMemberSerializers members, IConverters converters,
			                         IContentType type)
			{
				_content     = content;
				_serializer  = serializer;
				_identities  = identities;
				_serializers = serializers;
				_members     = members;
				_converters  = converters;
				_type        = type;
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
				var item     = CollectionItemTypeLocator.Default.Get(serializer.Profile.MemberType);
				var itemType = _type.Get(owner);
				var member   = (IMemberSerializer)new MemberSerializer(serializer, _converters.Get(itemType));
				var result = item != null
					             ? new ListSerializer(_serializer, _serializers, serializer, member,
					                                  _identities.Get(owner),
					                                  Generic.Instance.Get(item)
					                                         .Invoke(), item)
					             : member;
				return result;
			}
		}

		interface ILists : IParameterizedSource<ArrayList, IList> {}

		sealed class Generic : Generic<ILists>
		{
			public static Generic Instance { get; } = new Generic();

			Generic() : base(typeof(GenericList<>)) {}
		}

		sealed class GenericList<T> : ILists
		{
			public static GenericList<T> Instance { get; } = new GenericList<T>();

			GenericList() {}

			public IList Get(ArrayList parameter) => parameter.OfType<T>()
			                                                  .ToList();
		}

		sealed class ListSerializer : IMemberSerializer
		{
			readonly ISerializer                            _serializer;
			readonly ISerializers                           _serializers;
			readonly IMemberSerializer                      _default;
			readonly IMemberSerializer                      _item;
			readonly IIdentity                              _identity;
			readonly IParameterizedSource<ArrayList, IList> _lists;
			readonly TypeInfo                               _type;

			public ListSerializer(ISerializer serializer, ISerializers serializers, IMemberSerializer @default,
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
						var emitter = info == _type ? _item : _serializers.Get(info);
						emitter.Write(writer, item);
					}
				}
			}

			public IMember Profile => _item.Profile;

			public IMemberAccess Access => _item.Access;
		}

		sealed class InnerContentActivation : IInnerContentActivation
		{
			readonly IInnerContentActivation _activator;
			readonly IContentType            _type;

			public InnerContentActivation(IInnerContentActivation activator, IContentType type)
			{
				_activator = activator;
				_type      = type;
			}

			public IInnerContentActivator Get(TypeInfo parameter)
			{
				var content  = _activator.Get(parameter);
				var typeInfo = _type.Get(parameter);
				var result = typeInfo != null
					             ? new InnerContentActivator(content,
					                                         IsCollectionTypeSpecification
						                                         .Default.IsSatisfiedBy(typeInfo))
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

			public IInnerContent Get(IFormatReader parameter) => new InnerContent(_activator.Get(parameter), _apply);
		}

		interface IContentMembers : IParameterizedSource<IMemberSerialization, IMemberSerializer> {}

		sealed class ContentMembers : IContentMembers
		{
			public static ContentMembers Instance { get; } = new ContentMembers();

			ContentMembers() : this(IsContent.Instance.IsSatisfiedBy) {}

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

		sealed class IsContent : DecoratedSpecification<MemberInfo>
		{
			public static IsContent Instance { get; } = new IsContent();

			IsContent() : base(IsDefinedSpecification<XmlTextAttribute>.Default) {}
		}

		interface IContentType : IParameterizedSource<TypeInfo, TypeInfo> {}

		sealed class ContentType : ReferenceCacheBase<TypeInfo, TypeInfo>, IContentType
		{
			readonly static Func<MemberInfo, bool> Specification = IsContent.Instance.IsSatisfiedBy;

			readonly Func<MemberInfo, bool> _specification;
			readonly ITypeMembers           _members;

			public ContentType(ITypeMembers members) : this(Specification, members) {}

			public ContentType(Func<MemberInfo, bool> specification, ITypeMembers members)
			{
				_specification = specification;
				_members       = members;
			}

			protected override TypeInfo Create(TypeInfo parameter)
			{
				var members = _members.Get(parameter);
				for (var i = 0; i < members.Length; i++)
				{
					var member = members[i];
					if (_specification(member.Metadata))
					{
						var result = IsCollectionTypeSpecification.Default.IsSatisfiedBy(member.MemberType)
							             ? member.Metadata.GetCustomAttribute<XmlTextAttribute>()
							                     ?.Type?.GetTypeInfo() ?? member.MemberType
							             : member.MemberType;
						return result;
					}
				}

				return null;
			}
		}

		sealed class InnerContent : IInnerContent
		{
			readonly IInnerContent    _content;
			readonly ConditionMonitor _monitor;

			public InnerContent(IInnerContent content, bool apply) :
				this(content, apply ? new ConditionMonitor() : null) {}

			public InnerContent(IInnerContent content, ConditionMonitor monitor)
			{
				_content = content;
				_monitor = monitor;
			}

			public IFormatReader Get() => _content.Get();

			public bool MoveNext() => _content.MoveNext() || (_monitor?.Apply() ?? false);

			public void Reset()
			{
				_content.Reset();
			}

			public object Current => _content.Current;
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