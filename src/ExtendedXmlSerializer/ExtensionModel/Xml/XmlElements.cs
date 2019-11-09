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

		public bool MoveNext()
		{
			var isStartElement = _reader.Read() && _reader.IsStartElement() && _reader.Depth == _depth;
			return isStartElement;
		}

		public object Current => _reader;
	}
}