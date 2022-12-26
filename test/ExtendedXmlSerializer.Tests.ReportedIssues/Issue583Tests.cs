using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue583Tests
	{
		[Fact]
		public void Verify()
		{
			var inner = new InnerObject { Id = 100 };
			var subject = new RootObject
			{
				Item1 = inner,
				//Item2 = inner,
				List  = new List<InnerObject> {  }
			};

			new ConfigurationContainer().Create().Cycle(subject).Should().BeEquivalentTo(subject);
		}

		public class RootObject
		{
			public InnerObject Item1 { get; set; }

			public InnerObject Item2 { get; set; }

			public List<InnerObject> List { get; set; } = new();
		}

		public class InnerObject
		{
			public int Id { get; set; }
		}
	}
}