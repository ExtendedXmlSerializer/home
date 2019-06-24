using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Drawing;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue249Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new DataHolder();

			new ConfigurationContainer().EnableAllConstructors()
			                            .EnableParameterizedContent()
			                            .EnableReferences()
			                            .Create()
			                            .ForTesting()
			                            .Cycle(instance)
			                            .ShouldBeEquivalentTo(instance);
		}

		sealed class DataHolder
		{
			public Size Size { get; set; } = new Size(100, 200);
		}
	}
}