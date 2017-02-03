using System;
using System.Collections;
using System.Reflection;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	class XmlReader : IReader
	{
		readonly ITypeLocator _type;
		readonly System.Xml.XmlReader _reader;

		public XmlReader(System.Xml.XmlReader reader) : this(TypeLocator.Default, reader) {}

		public XmlReader(ITypeLocator type, System.Xml.XmlReader reader)
		{
			_type = type;
			_reader = reader;
		}

		public string DisplayName => _reader.LocalName;
		public TypeInfo Classification => _type.Get(_reader);

		public string Value()
		{
			_reader.Read();
			var result = _reader.Value;
			_reader.Read();
			return result;
		}

		public IEnumerator Members() => new Enumerator(_reader, _reader.Depth + 1);

		public IEnumerator Items() => new Enumerator(_reader, _reader.Depth + 1);

		sealed class Enumerator : IEnumerator
		{
			readonly System.Xml.XmlReader _reader;
			readonly int _depth;

			public Enumerator(System.Xml.XmlReader reader, int depth)
			{
				_reader = reader;
				_depth = depth;
			}

			public object Current => _reader;

			public bool MoveNext() => _reader.Read() && _reader.IsStartElement() && _reader.Depth == _depth;

			public void Reset()
			{
				throw new NotSupportedException();
			}
		}
	}
}