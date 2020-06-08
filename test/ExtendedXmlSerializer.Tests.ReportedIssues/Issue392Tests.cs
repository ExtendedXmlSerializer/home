using ExtendedXmlSerializer.Configuration;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue392Tests
	{
		readonly ITestOutputHelper _output;

		public Issue392Tests(ITestOutputHelper output) => _output = output;

		[Fact]
		void Verify()
		{
			var sw = Stopwatch.StartNew();
			new ConfigurationContainer().Create();
			_output.WriteLine($"{sw.ElapsedMilliseconds} ms");
		}
	}
}