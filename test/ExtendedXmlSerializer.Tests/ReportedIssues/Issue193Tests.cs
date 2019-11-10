using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ReflectionModel;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue193Tests
	{
		[Fact]
		void Registration()
		{
			var serializer = new ConfigurationContainer().Register<RandomType, SerializerWithDependencies>()
			                                             .Create()
			                                             .ForTesting();

			var instance = new RandomType
			{
				Description = "Dangerous territory. :)",
				Subject     = new Subject {Message = "Hello World! I haven't coded in forever yo!"}
			};
			var result =
				serializer.Assert(instance,
				                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue193Tests-RandomType DescriptionOverride=""Override: Dangerous territory. :)"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Subject><Message>Hello World! I haven't coded in forever yo!</Message></Subject></Issue193Tests-RandomType>");

			result.Description.Should()
			      .Be("Override: Dangerous territory. :)");
			result.Subject.Message.Should()
			      .Be(instance.Subject.Message);
		}

		[Fact]
		void Extension()
		{
			var serializer = new ConfigurationContainer().Extend(RandomTypeExtension.Default)
			                                             .Create()
			                                             .ForTesting();

			var instance = new RandomType
			{
				Description = "Dangerous territory. :)",
				Subject     = new Subject {Message = "Hello World! I haven't coded in forever yo!"}
			};
			var result =
				serializer.Assert(instance,
				                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue193Tests-RandomType DescriptionOverride=""Override: Dangerous territory. :)"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Description>Dangerous territory. :)</Description><Subject><Message>Hello World! I haven't coded in forever yo!</Message></Subject></Issue193Tests-RandomType>");

			result.Description.Should()
			      .Be("Override: Dangerous territory. :)");
			result.Subject.Message.Should()
			      .Be(instance.Subject.Message);
		}

		sealed class RandomTypeExtension : ISerializerExtension
		{
			public static RandomTypeExtension Default { get; } = new RandomTypeExtension();

			RandomTypeExtension() {}

			public IServiceRepository Get(IServiceRepository parameter) => parameter.Decorate<IContents, Contents>();

			public void Execute(IServices parameter) {}

			sealed class Contents : IContents
			{
				readonly IContents                _contents;
				readonly ISpecification<TypeInfo> _specification;

				[UsedImplicitly]
				public Contents(IContents contents) : this(contents, IsAssignableSpecification<RandomType>.Default) {}

				public Contents(IContents contents, ISpecification<TypeInfo> specification)
				{
					_contents      = contents;
					_specification = specification;
				}

				public ISerializer Get(TypeInfo parameter)
				{
					var serializer = _contents.Get(parameter);
					var result = _specification.IsSatisfiedBy(parameter)
						             ? new Serializer(serializer).Adapt()
						             : serializer;
					return result;
				}
			}

			sealed class Serializer : ISerializer<RandomType>
			{
				readonly ISerializer _serializer;

				public Serializer(ISerializer serializer) => _serializer = serializer;

				public RandomType Get(IFormatReader parameter)
				{
					var xml = parameter.Get()
					                   .To<System.Xml.XmlReader>();
					var description = xml.GetAttribute("DescriptionOverride");
					var result      = (RandomType)_serializer.Get(parameter);
					result.Description = description;
					return result;
				}

				public void Write(IFormatWriter writer, RandomType instance)
				{
					var xml = writer.Get()
					                .To<System.Xml.XmlWriter>();
					xml.WriteAttributeString("DescriptionOverride", $"Override: {instance.Description}");

					_serializer.Write(writer, instance);
				}
			}
		}

		sealed class RandomType
		{
			public string Description { get; set; }

			public Subject Subject { get; set; }
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}

		sealed class SerializerWithDependencies : ISerializer<RandomType>
		{
			readonly ISerializer _subject;

			[UsedImplicitly]
			public SerializerWithDependencies(IContents contents) : this(contents.Get(typeof(Subject))) {}

			SerializerWithDependencies(ISerializer subject) => _subject = subject;

			public RandomType Get(IFormatReader parameter)
			{
				var xml = parameter.Get()
				                   .To<System.Xml.XmlReader>();
				var description = xml.GetAttribute("DescriptionOverride");
				xml.Read();
				xml.MoveToContent();
				var subject = (Subject)_subject.Get(parameter);
				var result = new RandomType
				{
					Description = description,
					Subject     = subject
				};
				return result;
			}

			public void Write(IFormatWriter writer, RandomType instance)
			{
				var xml = writer.Get()
				                .To<System.Xml.XmlWriter>();
				xml.WriteAttributeString("DescriptionOverride", $"Override: {instance.Description}");

				xml.WriteStartElement("Subject");
				_subject.Write(writer, instance.Subject);
				xml.WriteEndElement();
			}
		}
	}
}