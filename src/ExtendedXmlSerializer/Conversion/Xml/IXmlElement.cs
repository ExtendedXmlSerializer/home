using ExtendedXmlSerialization.Conversion.Elements;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public interface IXmlElement : IElement
	{
		void Write(System.Xml.XmlWriter writer, object instance);
	}

	public interface IXmlWritable
	{
		void Write(System.Xml.XmlWriter writer, IElement element, object instance);
	}
}