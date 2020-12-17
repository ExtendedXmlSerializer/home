using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using FluentAssertions.Common;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue497Tests
	{
		[Fact]
		public void Verify()
		{
			IConfigurationContainer container  = new ConfigurationContainer();
			var                     properties = typeof(Subject).GetProperties();
			var                     type       = container.GetTypeConfiguration(typeof(Subject));
			foreach (var property in properties)
			{
				if (property.IsDecoratedWith<CustomAttribute>())
				{
					container = type.Get(property).Ignore(property);
				}
			}

			var sut      = container.Create().ForTesting();
			var instance = new Subject { Message = "Serialized", AnotherMessage = "Ignored" };
			var cycled   = sut.Cycle(instance);
			cycled.Message.Should().NotBeNullOrEmpty();
			cycled.AnotherMessage.Should().BeNull();
		}

		sealed class Subject
		{
			public string Message { get; set; }

			[Custom]
			public string AnotherMessage { get; set; }
		}

		sealed class CustomAttribute : Attribute {}
	}
}