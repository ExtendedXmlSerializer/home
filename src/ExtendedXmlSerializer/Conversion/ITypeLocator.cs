using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public interface ITypeLocator : IParameterizedSource<System.Xml.XmlReader, TypeInfo> {}
}