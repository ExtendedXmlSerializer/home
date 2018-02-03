using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

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

			var instance = new AnotherSubject { Subject = new Subject { Message = "Hello World!" } };
			serializer.Cycle(instance)
			          .ShouldBeEquivalentTo(instance); ;
		}

		interface ISubject {}

		sealed class Subject : ISubject
		{
			public string Message { get; set; }
		}

		sealed class AnotherSubject
		{
			public ISubject Subject { get; set; }
		}
	}
}
