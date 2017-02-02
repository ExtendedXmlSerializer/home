using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Options;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public class Converters : IConverters
	{
		readonly IParameterizedSource<TypeInfo, IConverter> _source;

		public Converters(IElements elements, IMemberAdorner adorner)
		{
			_source = new Selector<TypeInfo, IConverter>(new ConverterOptions(this, elements, adorner).ToArray());
		}

		public IConverter Get(TypeInfo parameter) => _source.Get(parameter);
	}
}