using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlReader : IFormatReader
	{
		readonly IFormatReaderContext _context;
		readonly System.Xml.XmlReader _reader;
		readonly string               _defaultNamespace;

		public XmlReader(IFormatReaderContext context, System.Xml.XmlReader reader)
			: this(context, reader, reader.LookupNamespace(string.Empty)) {}

		public XmlReader(IFormatReaderContext context, System.Xml.XmlReader reader, string defaultNamespace)
		{
			_context          = context;
			_reader           = reader;
			_defaultNamespace = defaultNamespace;
		}

		public string Name => _reader.LocalName;
		public string Identifier => _reader.NamespaceURI;

		public MemberInfo Get(string parameter) => _context.Get(parameter);

		public override string ToString() => $"{base.ToString()}: {IdentityFormatter.Default.Get(this)}";

		public bool IsSatisfiedBy(IIdentity parameter)
			=>
				_reader.HasAttributes &&
				_reader.MoveToAttribute(parameter.Name,
				                        parameter.Identifier == _defaultNamespace
					                        ? string.Empty
					                        : parameter.Identifier);

		public object Get() => _reader;

		public string Content()
		{
			switch (_reader.NodeType)
			{
				case XmlNodeType.Attribute:
					return _reader.Value;
				default:
					if (_reader.IsEmptyElement && _reader.CanReadValueChunk)
					{
						Set();
						return string.Empty;
					}

					var isNull = IsSatisfiedBy(NullValueIdentity.Default);

					if (!isNull)
					{
						_reader.Read();
					}

					var result = isNull ? null : _reader.Value;

					if (_reader.NodeType == XmlNodeType.CDATA)
					{
						return CharacterData(result);
					}
					else if (!string.IsNullOrEmpty(result))
					{
						_reader.Read();
						Set();
					}


					return result;
			}
		}

		string CharacterData(string result)
		{
			_reader.Read();
			Set();

			if (_reader.NodeType == XmlNodeType.CDATA)
			{
				var builder = new StringBuilder(result);
				while (_reader.NodeType == XmlNodeType.CDATA)
				{
					builder.Append(_reader.Value);
					_reader.Read();
					Set();
				}

				return builder.ToString();
			}

			return result;
		}

		public void Set() => _reader.MoveToContent();

		public void Dispose()
		{
			_reader.Dispose();
			_context.Dispose();
		}

		public IIdentity Get(string name, string identifier) => _context.Get(name, identifier);
	}
}