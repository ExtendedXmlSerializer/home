using ExtendedXmlSerialization.Conversion.Model.Names;

namespace ExtendedXmlSerialization.Conversion
{
	public class RootContext : NamedContext
	{
		public RootContext(IName name, IElementContext body) : base(name, body) {}
	}
}