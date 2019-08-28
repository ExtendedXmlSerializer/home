using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue261Tests
	{
		[Fact]
		void VerifyProperty()
		{
			var container = new ConfigurationContainer().EnableImplicitTypingFromNested<Issue261Tests>()
			                                            .EnableClassicSchemaTyping()
			                                            .Create()
			                                            .ForTesting();

			const string content = @"<?xml version=""1.0"" encoding=""utf-8""?><Issue261Tests-Reference xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Association xsi:type=""Issue261Tests-Project"" /></Issue261Tests-Reference>";
			container.Deserialize<Reference>(content).Association.Should()
			         .BeOfType<Project>();
		}

		[Fact]
		void VerifyElement()
		{
			var container = new ConfigurationContainer().EnableImplicitTypingFromNested<Issue261Tests>()
			                                            .EnableClassicSchemaTyping()
			                                            .Create()
			                                            .ForTesting();
			var content =
				@"<?xml version=""1.0"" encoding=""utf-8""?><List xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:arguments=""ModelObject"" xmlns=""https://extendedxmlserializer.github.io/system""><Capacity>4</Capacity><Issue261Tests-ModelObject xsi:type=""Issue261Tests-Project"" /></List>";
			container.Deserialize<List<Entity>>(content)
			         .Should()
			         .HaveCount(1)
			         .And.Subject.Only()
			         .Should()
			         .BeOfType<Project>();
		}

		[Serializable]
		[XmlType("ModelObject")]
		class Entity
		{
			public IList<Entity> Children { get; set;  } = new List<Entity>();
		}

		class Reference : Entity
		{
			public Entity Association { get; set; }
		}

		[Serializable]
		class Project : Entity {}

		[Serializable]
		class AnotherModelClass : Entity {}
	}
}