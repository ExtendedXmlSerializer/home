using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue565Tests_Extended
	{
		[Fact]
		public void Verify()
		{
			var subject  = new ConfigurationContainer().Create().ForTesting();
			var instance = new Store
			{
				[new byte[2]] = "First",
				[new byte[3]] = "Second",
				[new byte[4]] = "Third"
			};
			subject.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		sealed class Store : Dictionary<byte[], string>
		{
			public Store() : base(Issue565Tests_Extended.Comparer.Instance) {}
		}

		sealed class Comparer : EqualityComparer<byte[]>
		{
			public static Comparer Instance { get; } = new();

			Comparer() {}

			public override bool Equals(byte[] x, byte[] y) => x.Length == y.Length;

			public override int GetHashCode(byte[] obj) => obj.Length.GetHashCode();
		}
	}
}
