using ExtendedXmlSerializer.ContentModel.Content;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ExtendedXmlSerializer.Tests.ReportedIssues.Shared.Issue442
{
	[XmlRoot("Device", Namespace = "")]
	public class SiemensDeviceTemplate : IDeviceTemplate
	{
		[XmlAttribute("fb")]
		public string FB { get; set; }

		[Verbatim]
		[XmlElement("Definition")]
		public string Definition { get; set; }

		[Verbatim]
		[XmlElement("DeviceDataDb")]
		public string DeviceDataDb { get; set; }

		[Verbatim]
		[XmlElement("ServiceHandler")]
		public string ServiceHandler { get; set; }

		[Verbatim]
		[XmlElement("ParameterDb")]
		public string ParameterDb { get; set; }

		[Verbatim]
		[XmlElement(ElementName = "Releases")]
		public string Releases { get; set; }

		[XmlAttribute("commandStruct")]
		public string CommandStruct { get; set; }

		[Verbatim]
		[XmlElement("CommandStart")]
		public string CommandStart { get; set; }

		[XmlElement("WatchAndForceTable")]
		public List<WatchAndForceTableTemplate> WatchAndForceTableEntires { get; set; }
	}
}
