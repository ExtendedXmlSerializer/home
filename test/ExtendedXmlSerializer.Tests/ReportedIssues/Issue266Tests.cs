using ExtendedXmlSerializer.Configuration;
using System.IO;
using System.Xml;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
    public sealed class Issue266Tests
	{
		[Fact]
		void Verify()
		{
			var m = new XMLModMetadata {Author = "me", Name = "test", Description = "desc"};
			var ms = new MemoryStream();
			using (var xw = XmlWriter.Create(ms))
			{
				new ConfigurationContainer().Create()
											.Serialize(xw, m);
				xw.Flush();
			}
		}

		class XMLModMetadata {
			public string Author { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
		}
	}
}