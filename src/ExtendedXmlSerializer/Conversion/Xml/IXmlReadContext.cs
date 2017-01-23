using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.Read;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public interface IXmlReadContext : IReadContext
	{
		XElement Data { get; }
	}
}