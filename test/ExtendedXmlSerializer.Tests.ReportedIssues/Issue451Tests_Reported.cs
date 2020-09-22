using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Instances;
using FluentAssertions;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue451Tests_Reported
	{
		[Fact]
		public void TestInterceptor()
		{
			// language=XML
			const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
				<Issue451Tests_Reported-Processor>
				 <Enabled>true</Enabled>
				 <Filters>
				  <Filter>
				   <Type>ISO</Type>
				  </Filter>
				 </Filters>
				</Issue451Tests_Reported-Processor>";

			var serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(Processor))
														 .Type<Processor>()
														 .WithInterceptor(new Interceptor())
														 .Create();

			var       contentStream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
			using var reader        = XmlReader.Create(contentStream);
			var       processor     = (Processor)serializer.Deserialize(reader);
			processor.Should().NotBeNull();
			processor.Enabled.Should().BeTrue();
			processor.Filters.Should().NotBeEmpty();
			processor.Filters.Only().Type.Should().Be("ISO");
		}

		public class Interceptor : SerializationActivator
		{
			public override object Activating(Type instanceType)
			{
				// processor should be retrieved from IoC container, but created manually for simplicity of test
				var processor = new Processor(new Service());
				return processor;
			}
		}

		public interface IService {}

		class Service : IService {}

		public class Processor
		{
			// ReSharper disable once NotAccessedField.Local
			readonly IService _service;

			public bool Enabled { get; set; }

			public List<Filter> Filters { [UsedImplicitly] get; set; }

			public Processor(IService service)
			{
				_service = service;

				Filters = new List<Filter>();
			}
		}

		public class Filter
		{
			public string Type { get; set; }
		}
	}
}