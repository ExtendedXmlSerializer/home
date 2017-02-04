using System.Xml;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IEmitter
	{
		void Emit(XmlWriter writer, object instance);
	}
}