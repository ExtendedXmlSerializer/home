using ExtendedXmlSerialization.Conversion.Elements;

namespace ExtendedXmlSerialization.Conversion
{
	public class Root : Container
	{
		public Root(IElement element, IConverter body) : base(element, body) {}
	}
}