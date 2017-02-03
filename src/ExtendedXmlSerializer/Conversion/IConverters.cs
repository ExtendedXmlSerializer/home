using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IConverters : IParameterizedSource<TypeInfo, IConverter> {}
}