using System;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class ContentReader : IReader
	{
		readonly Func<string, object> _parser;

		public ContentReader(Func<string, object> parser) => _parser = parser;

		public object Get(IFormatReader parameter) => _parser(parameter.Content());
	}
}