using System;
using System.Xml.Serialization;

namespace ExtendedXmlSerializer.Tests.ReportedIssues.Shared
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace      = "http://namespace/file.xsd")]
	[XmlRoot(ElementName   = "FooBar", IsNullable = false)]
	public class Foo : IFoo
	{
		[XmlElement("num")]
		public int Number { get; set; }
	}

	public interface IFoo {}

	[Serializable, XmlType(AnonymousType = true, Namespace                         = "http://namespace/file.xsd"),
	 XmlRoot("myMessage", Namespace      = "http://namespace/file.xsd", IsNullable = false)]
	public class MyMessage
	{
		/// <remarks />
		[XmlElement("myElement")]
		public MyElementType MyElement { get; set; }
	}

	/// <remarks />
	[XmlType(Namespace = "http://namespace/file.xsd")]
	public class MyElementType
	{
		/// <remarks />
		[XmlAttribute("uniqueId")]
		public string UniqueId { get; set; }
	}

	[XmlType(Namespace = "")]
	public class None
	{
		/// <remarks />
		[XmlAttribute("uniqueId")]
		public string UniqueId { get; set; }
	}
}
