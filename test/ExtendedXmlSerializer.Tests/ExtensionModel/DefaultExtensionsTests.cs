using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel
{
	public sealed class DefaultExtensionsTests
	{
		[Fact]
		public void VerifyConvertible()
		{
			new ConfigurationRoot().ToSupport().Cycle(6776);
		}
	}
}