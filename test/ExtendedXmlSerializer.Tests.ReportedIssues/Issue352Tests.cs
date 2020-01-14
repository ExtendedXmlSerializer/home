using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

// ReSharper disable All

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue352Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new Config();
			var cycled = new ConfigurationContainer().Create()
			                                         .ForTesting()
			                                         .Cycle(instance);

			cycled.Should().BeEquivalentTo(instance);
		}

		public class Backend
		{
			public string Host { get; set; } = "localhost";
			public int Port { get; set; } = 8080;
		}

		public class Portal
		{
			public string Name { get; set; } = "portal";
			public Backend Backend { get; set; } = new Backend();
		}

		public class WorkerBackend
		{
			public string ServerUrl { get; set; } = "http://localhost:8080";
		}

		public class Worker
		{
			public string Name { get; set; } = "worker";
			public WorkerBackend Backend { get; set; } = new WorkerBackend();
		}

		public class Config
		{
			public Portal Portal { get; set; } = new Portal();
			public Worker Worker { get; set; } = new Worker();
		}
	}
}