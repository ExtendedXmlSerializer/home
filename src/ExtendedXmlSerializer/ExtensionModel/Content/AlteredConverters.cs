using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class AlteredConverters : IConverters
	{
		readonly Func<IConverter, IConverter> _alter;
		readonly IConverters                  _converters;

		public AlteredConverters(IAlteration<IConverter> alteration, IConverters converters)
			: this(alteration.Get, converters) {}

		public AlteredConverters(Func<IConverter, IConverter> alter, IConverters converters)
		{
			_alter      = alter;
			_converters = converters;
		}

		public IConverter Get(TypeInfo parameter) => _converters.Get(parameter).Alter(_alter);
	}
}