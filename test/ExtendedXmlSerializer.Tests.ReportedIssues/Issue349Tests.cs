using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue349Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer().Type<RootClass>()
			                                             .Register()
			                                             .Serializer()
			                                             .Composer()
			                                             .Of<Composer>()
			                                             .Member(x => x.SomeAttributeProperty)
			                                             .Attribute()
			                                             .Create()
			                                             .ForTesting();
			var instance = new RootClass
			{
				SomeAttributeProperty = "Hello World!", ManyToOneReference = new A {Name = "A"},
				OneToManyReference    = new B {Name                                      = "B"}
			};

			serializer.Assert(instance,
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue349Tests-RootClass Name=""A"" SomeAttributeProperty=""Hello World!"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues""><ManyToOneReference><Name>A</Name></ManyToOneReference><OneToManyReference><Name>B</Name></OneToManyReference></Issue349Tests-RootClass>");

			serializer.Cycle(instance)
			          .Should()
			          .BeEquivalentTo(instance);
		}

		sealed class Composer : ISerializerComposer<RootClass>
		{
			readonly IIdentity _name;

			[UsedImplicitly]
			public Composer(IIdentityStore store) : this(store.Get("Name", string.Empty)) {}

			public Composer(IIdentity name) => _name = name;

			public ISerializer<RootClass> Get(ISerializer<RootClass> parameter) => new Serializer(parameter, _name);
		}

		sealed class Serializer : ISerializer<RootClass>
		{
			readonly ISerializer<RootClass> _serializer;
			readonly IIdentity              _name;

			public Serializer(ISerializer<RootClass> serializer, IIdentity name)
			{
				_serializer = serializer;
				_name       = name;
			}

			public RootClass Get(IFormatReader parameter) => _serializer.Get(parameter);

			public void Write(IFormatWriter writer, RootClass instance)
			{
				writer.Content(_name, instance.ManyToOneReference.Name);
				_serializer.Write(writer, instance);
			}
		}

		public class RootClass
		{
			public string SomeAttributeProperty { get; set; }

			public A ManyToOneReference { get; set; }

			public B OneToManyReference { get; set; }
		}

		public class A
		{
			public string Name { get; set; }
		}

		public class B
		{
			public string Name { get; set; }
		}
	}
}