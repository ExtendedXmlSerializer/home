using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Xunit;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue181Tests
    {
		[Fact]
        public void TestDictionarySerialization()
        {
            var data = new Model();
            data.Init();
            var tempDir = Path.GetTempPath();
            var tempFile = Path.Combine(tempDir, "test.xml");
            var serializer = new ConfigurationContainer().UseOptimizedNamespaces().Create();
            using (XmlWriter writer = XmlWriter.Create(tempFile, new XmlWriterSettings { Indent = true }))
                serializer.Serialize(writer, data);

			using (var reader = new XmlReaderFactory().Get(File.OpenRead(tempFile)))
	        {
	            var result = (Model)serializer.Deserialize(reader);
				result.Map
				      .Should()
				      .Equal(data.Map);
	        }

			File.Delete(tempFile);

	        /*// throws:
	        using (var reader = XmlReader.Create(File.OpenRead(tempFile)))
	        {
		        var result = (Model)serializer.Deserialize(reader);
	        }*/
        }


	    class Model
	    {
		    public int Id { get; set; }
		    public string Name { get; set; }
		    public IDictionary<string, string> Map { get; set; } = new Dictionary<string, string>();

		    public void Init()
		    {
			    Id   = 2;
			    Name = "Parent";
			    Map  = new Dictionary<string, string> { { "test", "test1" }, { "test2", "test2" } };
		    }
	    }

    }
}
