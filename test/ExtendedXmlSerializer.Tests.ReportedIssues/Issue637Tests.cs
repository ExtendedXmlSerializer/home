#if CORE
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue637Tests
	{
		[Fact]
		public void Verify()
		{
			var sut      = new ConfigurationContainer().Create().ForTesting();
			var instance = new Class2();
			sut.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		[Fact]
		public void VerifyModified()
		{
			var sut      = new ConfigurationContainer().Create().ForTesting();
			var instance = new Class2 { List2 = [23]};
			sut.Cycle(instance).Should().BeEquivalentTo(instance);
		}
		
		sealed class Class2
		{
			public IReadOnlyList<int> List1 { get; set; } = [];                 //this works
			public IReadOnlyList<int> List2 { get; set; } = [1];                //this breaks Serialize&Deserialize
			public IList<int> List3 { get; set; } = [1,2];              //this works
			public IReadOnlyList<int> List4 { get; set; } = new List<int>(){1}; //this works (workaround)
		}

	}
}
#endif