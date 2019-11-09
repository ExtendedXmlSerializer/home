using System;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class ByteArrayConverter : Converter<byte[]>
	{
		public static ByteArrayConverter Default { get; } = new ByteArrayConverter();

		ByteArrayConverter() : base(Convert.FromBase64String, Convert.ToBase64String) {}
	}
}