using ExtendedXmlSerializer.Configuration;
using FluentAssertions;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue491Tests
	{
		[Fact]
		public void Test()
		{
			var serializer = new ConfigurationContainer().WithUnknownContent().Continue().Create();

			// Create an object which have its Id as element
			var obj2 = new RootObjectIdElement() {Id = 2, Name = "RootObjectId 2"};
			var text2 = serializer.Serialize(obj2);
			//_output.WriteLine(text2);
			// <?xml version="1.0" encoding="utf-8"?><BugAttributeInsteadOfElement-RootObjectIdElement xmlns="clr-namespace:Test.XmlSerializer;assembly=Test"><Id>2</Id><Name>RootObjectId 2</Name></BugAttributeInsteadOfElement-RootObjectIdElement>

			// TEST 1: Change object types which allows deserialize type
			var bugText1 = text2.Replace("RootObjectIdElement", "RootObjectIdAttribute");
			var bugObj1 = serializer.Deserialize<RootObjectIdAttribute>(bugText1);
			// RESULT 1: BUG encountered! Name is null although WithUnknownContent().Continue() is enabled
			bugObj1.Name.Should().Be("RootObjectId 2");
			bugObj1.Id.Should().Be(0);

			// TEST 2: Set Id as Attribute and parallel as unknown element
			var bugText2 = bugText1.Replace("-RootObjectIdAttribute ", @"-RootObjectIdAttribute Id=""1"" ");
			var bugObj2 = serializer.Deserialize<RootObjectIdAttribute>(bugText2);
			// RESULT 2: BUG also exists in case the attribute is set too
			bugObj2.Name.Should().Be("RootObjectId 2");
			bugObj2.Id.Should().Be(1);




			// TEST 3: Rename element Id to Id2 - therefore it has not the same name as expected attribute
			var bugText3 = bugText1.Replace("<Id>", "<Id2>").Replace("</Id>", "</Id2>");
			var bugObj3 = serializer.Deserialize<RootObjectIdAttribute>(bugText3);
			// RESULT 3: Works just fine
			bugObj3.Name.Should().NotBeNull();
			bugObj3.Id.Should().Be(0);

			// => BUG happens only if an unknown content is determined which has same name as an expected attribute!
		}

		public class RootObjectIdAttribute
		{
			[XmlAttribute]
			public int Id { get; set; }

			public string Name { get; set; }
		}

		public class RootObjectIdElement
		{
			public int Id { get; set; }

			public string Name { get; set; }
		}

	}
}
