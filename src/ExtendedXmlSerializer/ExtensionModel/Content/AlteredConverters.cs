using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class AlteredConverters : IConverters
	{
		readonly IAlteration<IConverter> _alteration;
		readonly IConverters             _converters;

		public AlteredConverters(IAlteration<IConverter> alteration, IConverters converters)
		{
			_alteration = alteration;
			_converters = converters;
		}

		public IConverter Get(TypeInfo parameter) => _converters.Get(parameter)
		                                                        .Alter(_alteration.Get);
	}
}