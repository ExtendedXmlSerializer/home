using ExtendedXmlSerializer.Tests.Support;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Conversion
{
	public class ByteArrayConverterTests
	{
		[Fact]
		public void Verify()
		{
			var instance = new byte[] {1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 2, 1};

			var support = new SerializationSupport();
			var actual =
				support.Assert(instance,
				               @"<?xml version=""1.0"" encoding=""utf-8""?><Array xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:item=""unsignedByte"" xmlns=""https://extendedxmlserializer.github.io/system"">AQIDBAUGBwcGBQQDAgE=</Array>");
			Assert.Equal(instance, actual);
		}

		[Fact]
		public void VerifyProperty()
		{
			var instance = new Subject {Bytes = new byte[] {1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 2, 1}};

			var support = new SerializationSupport();
			var actual  = support.Cycle(instance);
			Assert.Equal(instance.Bytes, actual.Bytes);
		}

		class Subject
		{
			public byte[] Bytes { get; set; }
		}
	}
}