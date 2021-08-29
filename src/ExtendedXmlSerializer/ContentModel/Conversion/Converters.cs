using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class Converters : IConverters
	{
		readonly IConverter[] _converters;

		public Converters(IConverter[] converters) => _converters = converters;

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