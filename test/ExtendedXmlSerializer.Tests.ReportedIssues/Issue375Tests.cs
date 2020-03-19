using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue375Tests
	{
		[Fact]
		public void Verify()
		{
			/*var instance = new Subject{ Inner = new Inner() };

			var serializer = new ConfigurationContainer().Create().ForTesting();

			serializer.Serialize(instance);

			serializer.Serialize(instance);*/

		}

		/*sealed class Subject
		{
			public Inner Inner { get; set; }
		}

		sealed class Inner {}*/
	}
}