using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

// ReSharper disable UnusedVariable

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue346Tests_Custom
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer().Type<RootClass>()
			                                             .Register()
			                                             .Serializer()
			                                             .Composer()
			                                             .ByCalling(x => new Serializer(x))
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
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue346Tests_Custom-RootClass Name=""A"" SomeAttributeProperty=""Hello World!"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues""><ManyToOneReference><Name>A</Name></ManyToOneReference><OneToManyReference><Name>B</Name></OneToManyReference></Issue346Tests_Custom-RootClass>");

			serializer.Cycle(instance)
			          .Should()
			          .BeEquivalentTo(instance);
		}

		[Fact]
		void VerifyWithAutoFormatting()
		{
			var serializer = new ConfigurationContainer().UseAutoFormatting()
			                                             .Type<RootClass>()
			                                             .Register()
			                                             .Serializer()
			                                             .Composer()
			                                             .ByCalling(x => new Serializer(x))
			                                             .Create()
			                                             .ForTesting();
			var instance = new RootClass
			{
				SomeAttributeProperty = "Hello World!", ManyToOneReference = new A {Name = "A"},
				OneToManyReference    = new B {Name                                      = "B"}
			};

			serializer.Assert(instance,
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue346Tests_Custom-RootClass Name=""A"" SomeAttributeProperty=""Hello World!"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues""><ManyToOneReference Name=""A"" /><OneToManyReference Name=""B"" /></Issue346Tests_Custom-RootClass>");

			serializer.Cycle(instance)
			          .Should()
			          .BeEquivalentTo(instance);
		}

		sealed class Serializer : ISerializer<RootClass>
		{
			readonly ISerializer<RootClass> _serializer;
			readonly IIdentity              _name;

			public Serializer(ISerializer<RootClass> serializer) : this(serializer, NameIdentity.Default) {}

			public Serializer(ISerializer<RootClass> serializer, IIdentity name)
			{
				_serializer = serializer;
				_name       = name;
			}

			public RootClass Get(IFormatReader parameter)
			{
				System.Xml.XmlReader native = parameter.Get().AsValid<System.Xml.XmlReader>(); // if you need it.
				return _serializer.Get(parameter);
			}

			public void Write(IFormatWriter writer, RootClass instance)
			{
				System.Xml.XmlWriter native = writer.Get().AsValid<System.Xml.XmlWriter>(); // if you need it.

				writer.Content(_name, instance.ManyToOneReference.Name);
				_serializer.Write(writer, instance);
			}

			sealed class NameIdentity : IIdentity
			{
				public static NameIdentity Default { get; } = new NameIdentity();

				NameIdentity() : this("Name", string.Empty) {}

				public NameIdentity(string name, string identifier)
				{
					Name       = name;
					Identifier = identifier;
				}

				public string Name { get; }

				public string Identifier { get; }
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