using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue470Tests
	{
		[Fact]
		public void Verify()
		{
			var instance = new Foo();
			new ConfigurationContainer().Create().ForTesting().Cycle(instance).Should().BeEquivalentTo(instance);
		}

		public class Foo
		{
			public string Bar { get; set; }
		}

		public class Something
		{
			[UsedImplicitly]
			Func<object, Task> Check<T>(Func<T, Task> data)
			{
				return async o => await data((T)o); // this causes it to bomb out
			}
		}
	}
}
