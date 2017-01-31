using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Names;
using ExtendedXmlSerialization.Conversion.Options;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public class Converters : IConverters
	{
		readonly IParameterizedSource<TypeInfo, IConverter> _source;

		public Converters(INames names)
		{
			_source = new Selector<TypeInfo, IConverter>(new ConverterOptions(this, names).ToArray());
		}

		public IConverter Get(TypeInfo parameter) => _source.Get(parameter);
	}
}