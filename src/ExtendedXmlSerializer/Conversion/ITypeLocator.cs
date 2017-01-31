using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public interface ITypeLocator : IParameterizedSource<XmlReader, TypeInfo> {}
}