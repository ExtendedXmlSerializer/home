using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Conversion
{
	public class UriConverterTests
	{
		[Fact]
		public void Verify()
		{
			var instance = new Uri("https://extendedxmlserializer.github.io");

			new SerializationSupport().Cycle(instance)
			                          .Should().BeEquivalentTo(instance);
		}
	}
}