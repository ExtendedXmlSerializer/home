using System;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class ContentWriter : IWriter
	{
		readonly Func<object, string> _formatter;

		public ContentWriter(Func<object, string> formatter) => _formatter = formatter;

		public void Write(IFormatWriter writer, object instance) => writer.Content(_formatter(instance));
	}
}