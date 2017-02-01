using ExtendedXmlSerialization.Conversion.Names;

namespace ExtendedXmlSerialization.Conversion
{
	public class Root : Container
	{
		public Root(IName name, IConverter body) : base(name, body) {}
	}
}