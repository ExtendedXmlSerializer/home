using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using Xunit;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace ExtendedXmlSerializer.Tests.Configuration
{
	public sealed class ContainerTests
	{
		[Fact]
		public void Verify()
		{
			var serializers = new ConfigurationContainer().ToSupport();
			serializers.Cycle(6776);
		}

		[Fact]
		public void VerifyClass()
		{
			var serializers = new ConfigurationContainer().ToSupport();
			serializers.Cycle(new Subject{ Message = "Hello World!"});
		}


		sealed class Subject
		{
			public string Message { get; set; }
		}
	}
}
