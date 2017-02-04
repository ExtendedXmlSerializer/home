using ExtendedXmlSerialization.Conversion.Elements;

namespace ExtendedXmlSerialization.Conversion
{
	public class Root : Container
	{
		public Root(IEmitter start, IConverter body) : base(start, body) {}
	}
}