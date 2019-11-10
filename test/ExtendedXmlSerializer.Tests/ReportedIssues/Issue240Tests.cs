using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue240Tests
	{
		[Fact]
		void Verify()
		{
			var subject = new ConfigurationContainer().Extend(DefaultListContentExtension.Default)
			                                          .Create()
			                                          .ForTesting();
			var instance = new Subject
			{
				Number   = 6776,
				Messages = new List<string> {"Hello", "World"}
			};

			subject.Cycle(instance)
			       .Should().BeEquivalentTo(instance);
		}

		[Fact]
		void VerifyComposite()
		{
			var subject = new ConfigurationContainer().Extend(DefaultListContentExtension.Default)
			                                          .Create()
			                                          .ForTesting();
			var inner = new Subject
			{
				Number   = 6776,
				Messages = new List<string> {"Hello", "World"}
			};

			var instance = new Container {Message = "Testing", Subject = inner, UnderSubject = 123};

			// This *shouldn't* be 0, but is due to the current v2 writing infrastructure.
			subject.Cycle(instance)
			       .UnderSubject.Should()
			       .Be(0);
		}

		[Fact]
		void VerifyRead()
		{
			const string content =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Issue240Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Message>Hello</Message><Message>World</Message></Issue240Tests-Subject>";

			var read = new ConfigurationContainer().Extend(DefaultListContentExtension.Default)
			                                       .Create()
			                                       .ForTesting()
			                                       .Deserialize<Subject>(content);
			var expected = new Subject {Messages = new List<string> {"Hello", "World"}};
			read.Should().BeEquivalentTo(expected);
			read.Messages.Count.Should()
			    .Be(2);
		}

		sealed class Subject
		{
			public int Number { [UsedImplicitly] get; set; }

			[XmlElement("Message")]
			public List<string> Messages { get; set; }
		}

		sealed class Container
		{
			public string Message { [UsedImplicitly] get; set; }

			public Subject Subject { [UsedImplicitly] get; set; }

			public int UnderSubject { get; set; }
		}

		sealed class DefaultListContentExtension : ISerializerExtension
		{
			public static DefaultListContentExtension Default { get; } = new DefaultListContentExtension();

			DefaultListContentExtension() {}

			public IServiceRepository Get(IServiceRepository parameter)
				=> parameter.RegisterInstance<IElementMember>(ElementMember.Instance)
				            .Register<IElementMemberContents, DefaultElementMemberContents>()
				            .RegisterInstance<IElementMembers>(ElementMembers.Instance)
				            .Decorate<IMemberSerializers, MemberSerializers>()
				            .Decorate<ITypeMemberSource, TypeMemberSource>();

			public void Execute(IServices parameter) {}

			sealed class TypeMemberSource : ITypeMemberSource
			{
				readonly ITypeMemberSource _source;
				readonly IElementMember    _member;

				public TypeMemberSource(ITypeMemberSource source, IElementMember member)
				{
					_source = source;
					_member = member;
				}

				public IEnumerable<IMember> Get(TypeInfo parameter)
				{
					var source = _source.Get(parameter);
					var name   = _member.Get(parameter);
					var result = name != null
						             ? source
							             .Select(x => x.Name == name
								                          ? new Member(x.Name, int.MaxValue, x.Metadata, x.MemberType,
								                                       x.IsWritable)
								                          : x)
						             : source;
					return result;
				}
			}

			sealed class MemberSerializers : IMemberSerializers
			{
				readonly IMemberSerializers     _members;
				readonly IMemberAccessors       _accessors;
				readonly IElementMemberContents _contents;
				readonly IElementMember         _member;

				// ReSharper disable once TooManyDependencies
				public MemberSerializers(IMemberSerializers members, IMemberAccessors accessors,
				                         IElementMemberContents contents, IElementMember member)
				{
					_members   = members;
					_accessors = accessors;
					_contents  = contents;
					_member    = member;
				}

				public IMemberSerializer Get(IMember parameter)
				{
					var name   = _member.Get(parameter.Metadata.ReflectedType);
					var result = parameter.Name == name ? Content(name, parameter) : _members.Get(parameter);
					return result;
				}

				IMemberSerializer Content(string name, IMember profile)
				{
					var start  = new Identity<object>(new Identity(name, profile.Identifier)).Adapt();
					var body   = _contents.Get(profile);
					var access = _accessors.Get(profile);
					var member = new MemberSerializer(profile, access, body,
					                                  new Serializer(body, new Enclosure(start, body).Adapt()));
					var result = new ListContentSerializer(member, name);
					return result;
				}
			}

			interface IElementMemberContents : IMemberContents {}

			sealed class DefaultElementMemberContents : IElementMemberContents
			{
				readonly ICollectionItemTypeLocator _locator;
				readonly ISerializer                _runtime;
				readonly IContents                  _contents;

				[UsedImplicitly]
				public DefaultElementMemberContents(ISerializer runtime, IContents contents)
					: this(CollectionItemTypeLocator.Default, runtime, contents) {}

				public DefaultElementMemberContents(ICollectionItemTypeLocator locator, ISerializer runtime,
				                                    IContents contents)
				{
					_locator  = locator;
					_runtime  = runtime;
					_contents = contents;
				}

				public ISerializer Get(IMember parameter)
				{
					var type    = _locator.Get(parameter.MemberType);
					var content = _contents.Get(type);
					var result = VariableTypeSpecification.Default.IsSatisfiedBy(type)
						             ? new Serializer(content, new VariableTypedMemberWriter(type, _runtime, content))
						             : content;
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

			sealed class ListContentSerializer : IMemberSerializer
			{
				readonly static Generic<ILists> Lists = new Generic<ILists>(typeof(GenericList<>));

				readonly IMemberSerializer                      _item;
				readonly string                                 _name;
				readonly IParameterizedSource<ArrayList, IList> _lists;

				public ListContentSerializer(IMemberSerializer item, string name)
					: this(item, name, CollectionItemTypeLocator.Default.Get(item.Profile.MemberType)) {}

				public ListContentSerializer(IMemberSerializer item, string name, TypeInfo type)
					: this(item, name, Lists.Get(type)()) {}

				public ListContentSerializer(IMemberSerializer item, string name, ILists lists)
				{
					_item  = item;
					_name  = name;
					_lists = lists;
				}

				public object Get(IFormatReader parameter)
				{
					var reader = parameter.Get()
					                      .To<System.Xml.XmlReader>();

					var items = new ArrayList();

					reader.MoveToContent();

					while (parameter.Name == _name)
					{
						var value = _item.Get(parameter);
						items.Add(value);
						reader.Read();
					}

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
							_item.Write(writer, item);
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

				public static ElementMember Instance { get; } = new ElementMember();

				ElementMember() : this(IsCollectionTypeSpecification.Default, Specification) {}

				public ElementMember(ISpecification<TypeInfo> collection, Func<MemberInfo, bool> specification)
				{
					_collection    = collection;
					_specification = specification;
				}

				protected override string Create(TypeInfo parameter)
				{
					var members = parameter.GetRuntimeProperties()
					                       .ToArray();
					for (var i = members.Length - 1; i >= 0; i--)
					{
						var member = members[i];
						if (_specification(member) && _collection.IsSatisfiedBy(member.PropertyType))
						{
							return member.GetCustomAttribute<XmlElementAttribute>()
							             .ElementName;
						}
					}

					return null;
				}
			}
		}
	}
}