using System;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlInnerContent : IInnerContent
	{
		readonly IFormatReader _reader;
		readonly XmlContent    _content;

		public XmlInnerContent(IFormatReader reader, object current, XmlContent content)
		{
			Current  = current;
			_reader  = reader;
			_content = content;
		}

		public object Current { get; }

		public IFormatReader Get() => _reader;

		public bool MoveNext()
		{
			var contents = _content;
			return contents.MoveNext();
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}
	}
}