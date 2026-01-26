namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	readonly struct XmlElements
	{
		readonly System.Xml.XmlReader _reader;
		readonly int                  _depth;

		public XmlElements(System.Xml.XmlReader reader, int depth)
		{
			_reader = reader;
			_depth  = depth;
		}

		public bool MoveNext() => _reader.Read() && _reader.IsStartElement() && _reader.Depth == _depth;

		public object Current => _reader;

		public bool IsEmpty() => _reader.IsEmptyElement;
	}
}