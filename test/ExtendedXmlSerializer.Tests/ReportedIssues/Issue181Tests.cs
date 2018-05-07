using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using XmlReader = System.Xml.XmlReader;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue181Tests
	{
		[Fact]
		public void TestDictionarySerialization()
		{
			var model = new Model();
			model.Init();
			var serializer = new ConfigurationContainer().Create();

			var content = Encoding.UTF8.GetBytes(serializer.Serialize(model));
			var stream = new MemoryStream(content);

			using (var reader = XmlReader.Create(stream))
			{
				serializer.Deserialize(reader).To<Model>().Map.Should().Equal(model.Map);
			}
		}


		sealed class Model
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
