using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	struct XmlAttributes
	{
		readonly System.Xml.XmlReader _reader;

		bool _complete;

		public XmlAttributes(System.Xml.XmlReader reader)
		{
			_reader   = reader;
			_complete = false;
		}

		public bool MoveNext()
		{
			if (!_complete)
			{
				switch (_reader.NodeType)
				{
					case XmlNodeType.Attribute:
						var result = _reader.MoveToNextAttribute();
						_complete = !result;
						return result;
					default:
						return _reader.MoveToFirstAttribute();
				}
			}

			return false;
		}

		public object Current => _reader;
	}
}