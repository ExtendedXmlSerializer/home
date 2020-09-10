using System.Xml.Serialization;

namespace ExtendedXmlSerializer.Tests.ReportedIssues.Shared.Issue442
{
	[XmlRoot("WatchAndForceTableEntry")]
	public class WatchAndForceTableTemplate
	{
		[XmlAttribute("name")]
		public string Name { get; set; }
	}
}