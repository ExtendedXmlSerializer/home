namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	readonly struct XmlContent
	{
		readonly XmlAttributes? _attributes;
		readonly XmlElements?   _elements;

		public XmlContent(XmlAttributes? attributes, XmlElements? elements)
		{
			_attributes = attributes;
			_elements   = elements;
		}

		public bool MoveNext()
		{
			var attributes = _attributes;
			var content    = _elements;
			var result     = (attributes?.MoveNext() ?? false) || (content?.MoveNext() ?? false);
			return result;
		}

		public object Current => null;
	}
}