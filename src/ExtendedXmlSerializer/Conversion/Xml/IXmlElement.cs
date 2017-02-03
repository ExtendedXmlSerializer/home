using ExtendedXmlSerialization.Conversion.Elements;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public interface IXmlElement : IElement
	{
		string Namespace { get; }
	}

}