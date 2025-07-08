using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues;

public sealed class Issue657Tests
{
	[Fact]
	public void Verify()
	{
		var subject = new ConfigurationContainer()
		       .EnableImplicitTyping(typeof(ClsItem), typeof(ClsArray))
		       .Type<ClsItem>().Register().Serializer().Using(new ClsItemSerializer()) //.CustomSerializer(new ClsItemSerializer())
		       .Create().ForTesting();
		var content =
			@"<?xml version=""1.0"" encoding=""utf-8""?><main xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues""><Capacity>4</Capacity><array><Capacity>4</Capacity><item><Dataname>tank</Dataname></item></array></main>";
		var instance = new ClsRoot() { new ClsArray() { new ClsItem(){ Dataname = "tank" } } };
		subject.Deserialize<ClsRoot>(content).Should().BeEquivalentTo(instance);

	}

	
	public interface IDataElement
	{
		string Dataname { get; set; }
	}

	[XmlRoot("main")]
	public class ClsRoot : List<IDataElement>
	{

	}
	[XmlType("array")]
	public class ClsArray : List<ClsItem>, IDataElement
	{
		public string Dataname { get ; set; }
	}
	[XmlType("item")]
	public class ClsItem : IDataElement
	{
		public string Dataname { get; set; }
	}


	class ClsItemSerializer : ISerializer<ClsItem>
	{
		public ClsItem Get(IFormatReader parameter) => new();

		public void Write(IFormatWriter writer, ClsItem instance) {}
	}
}

