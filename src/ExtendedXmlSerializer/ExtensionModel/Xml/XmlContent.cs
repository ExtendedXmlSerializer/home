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
			return content?.IsEmpty() ?? false ? (bool)content?.MoveNext() : Next(in attributes, in content);
		}

		static bool Next(in XmlAttributes? attributes, in XmlElements? content)
			=> (attributes?.MoveNext() ?? false) || (content?.MoveNext() ?? false);

		public object Current => null;
	}
}