using System.IO;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Xml
{
	public class XmlReaderFactoryTests
	{
		[Fact]
		public void Same()
		{
			var sut = new XmlReaderFactory();
			sut.Get(new MemoryStream())
			   .NameTable.Should()
			   .BeSameAs(sut.Get(new MemoryStream())
			                .NameTable);
		}
	}
}