using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Options
{
	public interface IConverterOption : IOption<TypeInfo, IConverter> {}
}