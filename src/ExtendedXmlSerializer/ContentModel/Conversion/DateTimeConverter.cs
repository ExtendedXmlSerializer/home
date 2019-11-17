using System;
using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	/// <summary>
	/// A converter for handling <see cref="DateTime"/> values in line with how classical serialization handles them.
	/// </summary>
	public sealed class DateTimeConverter : ConverterBase<DateTime>
	{
		/// <summary>
		/// A local variant of this converter.  This is called with <see cref="XmlDateTimeSerializationMode.Local"/>.
		/// </summary>
		public static DateTimeConverter Local { get; } = new DateTimeConverter(XmlDateTimeSerializationMode.Local);

		/// <summary>
		/// The default instance of this converter.
		/// </summary>
		public static DateTimeConverter Default { get; } = new DateTimeConverter();

		DateTimeConverter() : this(XmlDateTimeSerializationMode.RoundtripKind) {}

		readonly XmlDateTimeSerializationMode _mode;

		/// <summary>
		/// Creates a new instance with the provided mode.
		/// </summary>
		/// <param name="mode">The mode used to handle conversion for provided DateTime instances.</param>
		public DateTimeConverter(XmlDateTimeSerializationMode mode) => _mode = mode;

		/// <inheritdoc />
		public override DateTime Parse(string data) => XmlConvert.ToDateTime(data, _mode);

		/// <inheritdoc />
		public override string Format(DateTime instance) => XmlConvert.ToString(instance, _mode);
	}
}