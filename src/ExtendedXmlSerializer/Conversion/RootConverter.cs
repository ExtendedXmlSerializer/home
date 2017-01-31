using ExtendedXmlSerialization.Conversion.Names;

namespace ExtendedXmlSerialization.Conversion
{
	public class RootConverter : NamedConverter
	{
		public RootConverter(IName name, IConverter body) : base(name, body) {}
	}
}