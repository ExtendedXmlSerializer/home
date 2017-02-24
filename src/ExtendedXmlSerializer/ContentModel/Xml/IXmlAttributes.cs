using System.Xml;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	public interface IXmlAttributes : IIdentity
	{
		bool Contains(IIdentity identity);

		bool Next();
	}

	class XmlAttributes : IXmlAttributes
	{
		readonly System.Xml.XmlReader _reader;
		public XmlAttributes(System.Xml.XmlReader reader)
		{
			_reader = reader;
		}

		public string Identifier => _reader.Prefix;
		public string Name => _reader.Prefix;

		public bool Contains(IIdentity identity)
			=> _reader.HasAttributes && _reader.MoveToAttribute(identity.Name, identity.Identifier);

		public bool Next()
		{
			if (_reader.HasAttributes)
			{
				switch (_reader.NodeType)
				{
					case XmlNodeType.Attribute:
						return _reader.MoveToNextAttribute();
					default:
						return _reader.MoveToFirstAttribute();
				}
			}
			return false;
		}
	}
}