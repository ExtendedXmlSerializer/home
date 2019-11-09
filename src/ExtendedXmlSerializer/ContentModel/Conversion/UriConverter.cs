using System;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class UriConverter : Converter<Uri>
	{
		public static UriConverter Default { get; } = new UriConverter();

		UriConverter() : base(s => new Uri(s), uri => uri.ToString()) {}
	}
}