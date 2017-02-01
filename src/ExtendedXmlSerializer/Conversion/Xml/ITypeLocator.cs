using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public interface ITypeLocator : IParameterizedSource<System.Xml.XmlReader, TypeInfo> {}
}