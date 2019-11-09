using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue191Tests
	{
		[Fact]
		void Verify()
		{
			var test = new Test {Attribute = "aaa", Element = "bbb"};
			new ConfigurationContainer().InspectingType<Test>()
			                            .Create()
			                            .ForTesting()
			                            .Assert(test,
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Test Attribute=""aaa"" xmlns=""http://test.org/schema/test""><Element>bbb</Element></Test>");
		}

		[XmlType(Namespace = "http://test.org/schema/test", TypeName   = "Test"),
		 XmlRoot(Namespace = "http://test.org/schema/test", IsNullable = false)]
		public class Test
		{
			[XmlAttribute]
			public string Attribute { get; set; }

			[XmlElement]
			public string Element { get; set; }
		}
	}
}