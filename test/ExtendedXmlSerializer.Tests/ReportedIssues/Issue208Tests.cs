using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue208Tests
	{
		[Fact]
		void Verify()
		{
			var container = new ConfigurationContainer().EnableThreadProtection()
			                                            .Create()
			                                            .ForTesting();

			var subject = "Hello World!";

			var loop = Parallel.For(0, 1000,
			                        index => container.Cycle(subject)
			                                          .Should()
			                                          .Be(subject));
			while (!loop.IsCompleted)
			{
				Thread.Sleep(100);
			}
		}
	}
}