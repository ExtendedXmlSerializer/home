using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class Converters : IConverters
	{
		readonly IEnumerable<IConverter> _converters;

		public Converters(IEnumerable<IConverter> converters) => _converters = converters;

		public IConverter Get(TypeInfo parameter)
		{
			foreach (var converter in _converters)
			{
				if (converter.IsSatisfiedBy(parameter))
				{
					return converter;
				}
			}

			return null;
		}
	}
}