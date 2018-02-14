using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace ExtendedXmlSerializer.Tests.Configuration
{
	public sealed class ContainerTests
	{
		[Fact]
		public void Verify()
		{
			new ConfigurationContainer().ToSupport().Cycle(6776);
		}

		[Fact]
		public void VerifyTypes()
		{
			var container = new ConfigurationContainer();
			container.Types()
			         .Should()
			         .BeEmpty();

			container.Type<Subject>()
			         .Should()
			         .BeSameAs(container.Type<Subject>());

			container.Types()
			         .Should()
			         .ContainSingle();
		}

		[Fact]
		public void VerifyMembers()
		{
			var container = new ConfigurationContainer();
			var type = container.Type<Subject>();

			type.Members()
			    .Should()
			    .BeEmpty();

			var member = type.Member(x => x.Message);
			member.Should()
			      .BeSameAs(type.Member(x => x.Message));

			type.Members()
			    .Should()
			    .ContainSingle();
		}

		[Fact]
		public void VerifyClass()
		{
			new ConfigurationContainer().ToSupport()
			                            .Cycle(new Subject {Message = "Hello World!"});
		}

		[Fact]
		public void VerifyArray()
		{
			new ConfigurationContainer().ToSupport()
			                            .Cycle(new[]{1, 2, 3});
		}

		[Fact]
		public void VerifyCollection()
		{
			new ConfigurationContainer().ToSupport()
			                            .Cycle(new List<string>{ "Hello", "World"});
		}

		[Fact]
		public void VerifyDictionary()
		{
			new ConfigurationContainer().ToSupport()
			                            .Cycle(new Dictionary<string, int>{ {"Hello", 1}, {"World!", 2}});
		}


		[Fact]
		public void VerifyNullable()
		{
			var support = new ConfigurationContainer().ToSupport();
			support.Cycle(new int?(6776));
			support.Cycle((int?)null);
		}

		[Fact]
		public void VerifyGroups()
		{
			var container = new GroupContainer<ISerializerExtension>(Groups.Default);

			var all = container.ToArray();
			all.Should()
			   .HaveCount(16);

			all.First()
			   .Should()
			   .BeOfType<DefaultReferencesExtension>();
			all.Last()
			   .Should()
			   .BeSameAs(CachingExtension.Default);

			var types = container.Get(Groups.TypeSystem);
			types.Should()
			     .HaveCount(6);
			types.First()
			     .Should()
			     .BeOfType<TypeModelExtension>();
			types.Last()
			     .Should()
			     .BeOfType<MemberModelExtension>();
		}


		sealed class Subject
		{
			public string Message { get; set; }
		}

		sealed class Groups : ItemsBase<IGroup<ISerializerExtension>>
		{
			public static GroupName Start = new GroupName("Start"),
									TypeSystem = new GroupName("Type System"),
									Framework = new GroupName("Framework"),
									Elements = new GroupName("Elements"),
									Content = new GroupName("Content"),
									Format = new GroupName("Format"),
									Caching = new GroupName("Caching"),
									Finish = new GroupName("Finish");

			public static Groups Default { get; } = new Groups();
			Groups() : this(DefaultMetadataSpecification.Default, DefaultMemberOrder.Default) { }

			readonly IMetadataSpecification _metadata;
			readonly IParameterizedSource<MemberInfo, int> _defaultMemberOrder;

			public Groups(IMetadataSpecification metadata,
			              IParameterizedSource<MemberInfo, int> defaultMemberOrder)
			{
				_metadata = metadata;
				_defaultMemberOrder = defaultMemberOrder;
			}

			public override IEnumerator<IGroup<ISerializerExtension>> GetEnumerator()
			{
				yield return new Group<ISerializerExtension>(Start,
															 new DefaultReferencesExtension()
															 );

				yield return new Group<ISerializerExtension>(TypeSystem,
															 TypeModelExtension.Default,
															 SingletonActivationExtension.Default,
															 new MemberNamesExtension(),
															 new MemberOrderingExtension(_defaultMemberOrder),
															 ImmutableArrayExtension.Default,
															 MemberModelExtension.Default
															 );
				yield return new Group<ISerializerExtension>(Framework,
															 SerializationExtension.Default);
				yield return new Group<ISerializerExtension>(Elements);
				yield return new Group<ISerializerExtension>(Content,
															 Contents.Default,
															 ContentModelExtension.Default,
															 new AllowedMembersExtension(_metadata),
															 new AllowedMemberValuesExtension(),
															 new ConvertersExtension()
															 );
				yield return new Group<ISerializerExtension>(Format,
															 new XmlSerializationExtension(),
															 new MemberFormatExtension()
															 );
				yield return new Group<ISerializerExtension>(Caching, CachingExtension.Default);
				yield return new Group<ISerializerExtension>(Finish);
			}
		}
	}
}
