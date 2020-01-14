namespace ExtendedXmlSerializer.Tests.ReportedIssues.Shared
{
	class Backend {}

	class WorkerBackend {}

	class Worker
	{
		public string Name { get; set; } = "worker";
		public WorkerBackend Backend { get; set; } = new WorkerBackend();
	}
}
