using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Format;
using System;
using System.Collections;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlListInnerContent : IListInnerContent
	{
		readonly IFormatReader _reader;
		readonly XmlContent    _content;

		// ReSharper disable once TooManyDependencies
		public XmlListInnerContent(IFormatReader reader, object current, IList list, XmlContent content)
		{
			Current  = current;
			List     = list;
			_reader  = reader;
			_content = content;
		}

		public object Current { get; }

		public IList List { get; }

		public IFormatReader Get() => _reader;

		public bool MoveNext()
		{
			var contents = _content;
			return contents.MoveNext();
		}

		public void Reset() => throw new NotSupportedException();
	}
}