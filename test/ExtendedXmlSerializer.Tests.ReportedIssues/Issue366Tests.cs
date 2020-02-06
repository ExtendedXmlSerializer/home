using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Drawing;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue366Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new Model();
			new ConfigurationContainer().Create()
			                            .ForTesting()
			                            .Cycle(instance)
			                            .Should()
			                            .BeEquivalentTo(instance);
		}

		public class Model
		{
			public Model()
			{
				NestedModel = new NestedModel();
			}

			public NestedModel NestedModel { get; set; }
		}

		public class NestedModel
		{
			public Size Size { get; set; }
		}
	}
}