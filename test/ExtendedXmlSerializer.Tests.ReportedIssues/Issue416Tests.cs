using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue416Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().Type<SubjectA>()
			                                             .Member(x => x.Signature)
			                                             .Ignore() // <-- only configuring SubjectA, not SubjectB
			                                             .Create()
			                                             .ForTesting();

			var subjects = new List<ISubject>
			{
				new SubjectA {Signature = "testa"},
				new SubjectB {Signature = "testb"}
			};

			var cycled = serializer.Cycle(subjects);
			cycled[0].Signature.Should().BeNull();
			cycled[1].Signature.Should().Be("testb");
		}

		interface ISubject
		{
			string Signature { get; set; }
		}

		public abstract class BaseSubject : ISubject
		{
			public string Signature { get; set; }
		}

		public class SubjectA : BaseSubject {}

		public class SubjectB : BaseSubject {}
	}
}