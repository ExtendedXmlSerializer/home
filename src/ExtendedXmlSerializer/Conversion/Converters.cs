using System;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Options;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public class Converters : IConverters
	{
		readonly IParameterizedSource<TypeInfo, IConverter> _source;

		public Converters(Func<IConverters, IConverterOptions> options)
		{
			_source = new OptionSelector<TypeInfo, IConverter>(options(this).ToArray());
		}

		public IConverter Get(TypeInfo parameter) => _source.Get(parameter);
	}
}