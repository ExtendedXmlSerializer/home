using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class WellKnownConverters : Items<IConverter>
	{
		public static WellKnownConverters Default { get; } = new WellKnownConverters();

		WellKnownConverters() : base(
		                             BooleanConverter.Default,
		                             CharacterConverter.Default,
		                             ByteConverter.Default,
		                             ByteArrayConverter.Default,
		                             UnsignedByteConverter.Default,
		                             ShortConverter.Default,
		                             UnsignedShortConverter.Default,
		                             IntegerConverter.Default,
		                             UnsignedIntegerConverter.Default,
		                             LongConverter.Default,
		                             UnsignedLongConverter.Default,
		                             FloatConverter.Default,
		                             DoubleConverter.Default,
		                             DecimalConverter.Default,
		                             DateTimeConverter.Default.Adapt(),
		                             DateTimeOffsetConverter.Default,
		                             StringConverter.Default,
		                             GuidConverter.Default,
		                             TimeSpanConverter.Default,
		                             UriConverter.Default
		                            ) {}
	}
}