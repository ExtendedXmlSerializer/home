using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue584Tests
	{
		[Fact]
		public void Verify()
		{
			var items = new[] { "Some" };
			var same    = new InnerObject { Items = items };
			var instance = new RootObject
			{
				Item1 = same,
				Item2 = new InnerObject2 { Items = items },
			};

			var action = () =>
			             {
				             var serializer = new ConfigurationContainer()
				                              .Create()
				                              .ForTesting();
				             serializer.Cycle(instance).Should().BeEquivalentTo(instance);
			             };
			action.Should().Throw<Exception>().Where(x => x.GetType().Name == "MultipleReferencesDetectedException");
		}

		[Fact]
		public void VerifyConfiguration()
		{
			var items = new[] { "Some" };
			var same  = new InnerObject { Items = items };
			var instance = new RootObject
			{
				Item1 = same,
				Item2 = new InnerObject2 { Items = items },
			};

			var serializer = new ConfigurationContainer().AllowMultipleReferences()
			                                             .Create()
			                                             .ForTesting();
			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		public class RootObject
		{
			public InnerObject Item1 { get; set; }
			public InnerObject2 Item2 { get; set; }
		}

		public class InnerObject
		{
			public string[] Items { get; set; }
		}

		public class InnerObject2
		{
			public string[] Items { get; set; }
		}
	}
}