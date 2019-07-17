using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Xunit;
using ISerializers = ExtendedXmlSerializer.ContentModel.Content.ISerializers;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue240Tests
	{
		[Fact]
		void VerifyRead()
		{
			const string content =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Issue240Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><string xmlns=""https://extendedxmlserializer.github.io/system"">Hello</string><string xmlns=""https://extendedxmlserializer.github.io/system"">World</string></Issue240Tests-Subject>";

			var read = new ConfigurationContainer().Extend(DefaultListContentExtension.Default)
			                                       .Create()
			                                       .ForTesting()
			                                       .Deserialize<Subject>(content);
			var expected = new Subject{ Messages = new List<string>{ "Hello", "World" } };
			read.ShouldBeEquivalentTo(expected);
		}

		sealed class Subject
		{
			[XmlElement("string")]
			public List<string> Messages { get; set; }
		}

		sealed class DefaultListContentExtension : ISerializerExtension
		{
			public static DefaultListContentExtension Default { get; } = new DefaultListContentExtension();

			DefaultListContentExtension() {}

			public IServiceRepository Get(IServiceRepository parameter)
				=> parameter.Register<IElementMember, ElementMember>()
				            .RegisterInstance<IElementMembers>(ElementMembers.Instance)
				            .Decorate<IMemberSerializations, MemberSerializations>()
			/*.Decorate<IMemberSerializers, MemberSerializers>()*/;

			public void Execute(IServices parameter) {}

			sealed class MemberSerializations : IMemberSerializations
			{
				readonly IMemberSerializations _serialization;
				readonly ISerializer           _serializer;
				readonly ISerializers          _serializers;
				readonly IElementMembers       _element;
				readonly IElementMember        _name;

				public MemberSerializations(IMemberSerializations serialization, ISerializer serializer,
				                            ISerializers serializers, IElementMembers element, IElementMember name)
				{
					_serialization = serialization;
					_serializer    = serializer;
					_serializers   = serializers;
					_element       = element;
					_name          = name;
				}

				public IMemberSerialization Get(TypeInfo parameter)
				{
					var serialization = _serialization.Get(parameter);
					var content       = _element.Get(serialization);
					var result = content != null
						             ? new MemberSerialization(serialization,
						                                       new ListContentSerializer(_serializer, _serializers,
						                                                                 content, _name.Get(parameter)))
						             : serialization;
					return result;
				}
			}

			sealed class MemberSerialization : IMemberSerialization
			{
				readonly IMemberSerialization _serialization;
				readonly IMemberSerializer    _content;

				public MemberSerialization(IMemberSerialization serialization, IMemberSerializer content)
				{
					_serialization = serialization;
					_content       = content;
				}

				public ImmutableArray<IMemberSerializer> Get() => _serialization.Get();

				public IMemberSerializer Get(string parameter)
					=> parameter == _content.Profile.Name ? _content : _serialization.Get(parameter);

				public ImmutableArray<IMemberSerializer> Get(object parameter) => _serialization.Get(parameter);
			}

			interface ILists : IParameterizedSource<ArrayList, IList> {}

			sealed class GenericList<T> : ILists
			{
				public static GenericList<T> Instance { get; } = new GenericList<T>();

				GenericList() {}

				public IList Get(ArrayList parameter) => parameter.OfType<T>()
				                                                  .ToList();
			}

			sealed class ListContentSerializer : IMemberSerializer
			{
				readonly static Generic<ILists> Lists = new Generic<ILists>(typeof(GenericList<>));

				readonly ISerializer                            _serializer;
				readonly ISerializers                           _serializers;
				readonly IMemberSerializer                      _item;
				readonly string                                 _name;
				readonly IParameterizedSource<ArrayList, IList> _lists;
				readonly TypeInfo                               _type;

				public ListContentSerializer(ISerializer serializer, ISerializers serializers, IMemberSerializer item,
				                             string name)
					: this(serializer, serializers, item, name,
					       CollectionItemTypeLocator.Default.Get(item.Profile.MemberType)) {}

				public ListContentSerializer(ISerializer serializer, ISerializers serializers, IMemberSerializer item,
				                             string name, TypeInfo type)
					: this(serializer, serializers, item, name, Lists.Get(type)(), type) {}

				public ListContentSerializer(ISerializer serializer, ISerializers serializers, IMemberSerializer item,
				                             string name, ILists lists, TypeInfo type)
				{
					_serializer  = serializer;
					_serializers = serializers;
					_item        = item;
					_name        = name;
					_lists       = lists;
					_type        = type;
				}

				public object Get(IFormatReader parameter)
				{
					var reader = parameter.Get()
					                      .To<System.Xml.XmlReader>();

					var items = new ArrayList();

					while (parameter.Name == _name)
					{
						var value = _serializer.Get(parameter);
						items.Add(value);
						reader.Read();
						reader.MoveToContent();
					}

					reader.Read();

					var result = _lists.Get(items);
					return result;
				}

				public void Write(IFormatWriter writer, object instance)
				{
					var list = _item.Access.Get(instance)
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

			interface IElementMembers : IParameterizedSource<IMemberSerialization, IMemberSerializer> {}

			sealed class ElementMembers : IElementMembers
			{
				public static ElementMembers Instance { get; } = new ElementMembers();

				ElementMembers() : this(IsCollectionTypeSpecification.Default,
				                        IsDefinedSpecification<XmlElementAttribute>.Default.IsSatisfiedBy) {}

				readonly ISpecification<TypeInfo> _collection;
				readonly Func<MemberInfo, bool>   _specification;

				public ElementMembers(ISpecification<TypeInfo> collection, Func<MemberInfo, bool> specification)
				{
					_collection    = collection;
					_specification = specification;
				}

				public IMemberSerializer Get(IMemberSerialization parameter)
				{
					var members = parameter.Get();

					for (var i = members.Length - 1; i >= 0; i--)
					{
						var member = members[i];
						if (_specification(member.Profile.Metadata) &&
						    _collection.IsSatisfiedBy(member.Profile.MemberType))
						{
							return member;
						}
					}

					return null;
				}
			}

			interface IElementMember : IParameterizedSource<TypeInfo, string> {}

			sealed class ElementMember : ReferenceCacheBase<TypeInfo, string>, IElementMember
			{
				readonly static Func<MemberInfo, bool> Specification
					= IsDefinedSpecification<XmlElementAttribute>.Default.IsSatisfiedBy;

				readonly ISpecification<TypeInfo> _collection;
				readonly Func<MemberInfo, bool>   _specification;
				readonly ITypeMembers             _members;

				public ElementMember(ITypeMembers members)
					: this(IsCollectionTypeSpecification.Default, Specification, members) {}

				public ElementMember(ISpecification<TypeInfo> collection, Func<MemberInfo, bool> specification,
				                     ITypeMembers members)
				{
					_collection    = collection;
					_specification = specification;
					_members       = members;
				}

				protected override string Create(TypeInfo parameter)
				{
					var members = _members.Get(parameter);
					for (var i = members.Length - 1; i >= 0; i--)
					{
						var member = members[i];
						if (_specification(member.Metadata) && _collection.IsSatisfiedBy(member.MemberType))
						{
							return member.Metadata.GetCustomAttribute<XmlElementAttribute>()
							             .ElementName;
						}
					}

					return null;
				}
			}
		}
	}
}