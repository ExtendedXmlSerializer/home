using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Reflection;
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
			                                            .Extend(Extension.Default)
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
			                                            .Extend(Extension.Default)
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
			List<Entity> Children { get; } = new List<Entity>();
		}

		class Reference : Entity
		{
			public Entity Association { get; set; }
		}

		[Serializable]
		class Project : Entity {}

		[Serializable]
		class AnotherModelClass : Entity {}

		sealed class SchemaType : ExtendedXmlSerializer.ContentModel.Identification.Identity
		{
			public static SchemaType Default { get; } = new SchemaType();

			SchemaType() : base("type", "http://www.w3.org/2001/XMLSchema-instance") {}
		}

		sealed class Reader : TypedParsingReader
		{
			public static Reader Default { get; } = new Reader();

			Reader() : base(SchemaType.Default) {}
		}

		sealed class Extension : ISerializerExtension
		{
			public static Extension Default { get; } = new Extension();

			Extension() {}

			public IServiceRepository Get(IServiceRepository parameter)
				=> parameter.Decorate<IClassification, Classification>();

			void ICommand<IServices>.Execute(IServices parameter) {}

			sealed class Classification : IClassification
			{
				readonly IClassification   _classification;
				readonly IReader<TypeInfo> _reader;

				public Classification(IClassification classification) : this(classification, Reader.Default) {}

				public Classification(IClassification classification, IReader<TypeInfo> reader)
				{
					_classification = classification;
					_reader         = reader;
				}

				public TypeInfo Get(IFormatReader parameter) => parameter.IsSatisfiedBy(SchemaType.Default)
					                                                ? _reader.Get(parameter)
					                                                : _classification.Get(parameter);
			}
		}
	}
}