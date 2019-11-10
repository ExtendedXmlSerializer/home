using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue163Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().EnableThreadProtection()
			                                             .Create()
			                                             .ForTesting();

			var instance = new AnotherSubject {Inner = new Subject {Message = "Hello World!"}};
			serializer.Cycle(instance)
			          .Should().BeEquivalentTo(instance);
		}

		interface ISubject {}

		sealed class Subject : ISubject
		{
			public string Message { get; set; }
		}

		sealed class AnotherSubject
		{
			public ISubject Inner { get; set; }
		}
	}
}