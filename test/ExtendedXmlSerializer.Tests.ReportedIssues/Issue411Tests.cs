using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue411Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().Type<SubjectBase>()
			                                             .Member(x => x.Message)
			                                             .EmitWhen(x => false)
			                                             .Create()
			                                             .ForTesting();

			serializer.Cycle(new Subject {Message = "Hello World!"}).Message.Should().BeNull();
		}

		sealed class Subject : SubjectBase
		{
			public override string Message { get; set; }
		}

		public abstract class SubjectBase
		{
			public abstract string Message { get; set; }
		}
	}
}