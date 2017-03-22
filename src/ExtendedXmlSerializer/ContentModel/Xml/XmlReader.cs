// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Xml
{
	sealed class XmlReader : IXmlReader
	{
		readonly ClassificationSource _classification;

		readonly System.Xml.XmlReader _reader;

		public XmlReader(IGenericTypes genericTypes, ITypes types, ITypeProperty type, IItemTypeProperty item,
		                 IArgumentsProperty arguments, System.Xml.XmlReader reader)
		{
			switch (reader.MoveToContent())
			{
				case XmlNodeType.Element:
					_reader = reader;
					_classification = new ClassificationSource(genericTypes, types, type, item, arguments, this, _reader);
					break;
				default:
					throw new InvalidOperationException($"Could not locate the content from the Xml reader '{reader}.'");
			}
		}

		public string Name => _reader.LocalName;
		public string Identifier => _reader.NamespaceURI;

		public bool IsMember() => _reader.Prefix == string.Empty; // TODO: Probably a better indicator for this?

		public TypeInfo Classification
		{
			get
			{
				var source = _classification;
				return source.Get();
			}
		}

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

		public string Value()
		{
			switch (_reader.NodeType)
			{
				case XmlNodeType.Attribute:
					return _reader.Value;
				default:
					_reader.Read();
					var result = _reader.Value;
					Read();
					return result;
			}
		}


		public int? New()
		{
			if (_reader.HasAttributes && _reader.NodeType == XmlNodeType.Attribute)
			{
				Reset();
			}

			var result = !_reader.IsEmptyElement ? (int?) (_reader.Depth + 1) : null;
			return result;
		}

		public void Reset() => _reader.MoveToElement();

		public bool Next(int depth) => Read() && _reader.IsStartElement() && _reader.Depth == depth;

		bool Read()
		{
			var result = _reader.Read();
			if (result)
			{
				var source = _classification;
				source.Clear();
			}
			return result;
		}

		public string Get(string parameter) => _reader.LookupNamespace(parameter);

		public override string ToString()
			=> $"{base.ToString()}: {XmlQualifiedName.ToString(_reader.LocalName, _reader.NamespaceURI)}";

		public void Dispose() => _reader.Dispose();
		public System.Xml.XmlReader Get() => _reader;

		struct ClassificationSource : ISource<TypeInfo>
		{
			readonly static IdentityStore IdentityStore = IdentityStore.Default;

			TypeInfo _classification;

			readonly IXmlReader _owner;
			readonly System.Xml.XmlReader _reader;
			readonly ITypeProperty _type, _item;
			readonly IArgumentsProperty _arguments;
			readonly IIdentityStore _identities;
			readonly ITypes _generic, _types;

			public ClassificationSource(IGenericTypes genericTypes, ITypes types, ITypeProperty type, IItemTypeProperty item,
			                            IArgumentsProperty arguments, IXmlReader owner, System.Xml.XmlReader reader)
				: this(owner, reader, type, item, arguments, IdentityStore, genericTypes, types) {}

			ClassificationSource(IXmlReader owner, System.Xml.XmlReader reader, ITypeProperty type, ITypeProperty item,
			                     IArgumentsProperty arguments, IIdentityStore identities, ITypes generic,
			                     ITypes types)
			{
				_owner = owner;
				_reader = reader;
				_type = type;
				_item = item;
				_arguments = arguments;
				_identities = identities;
				_generic = generic;
				_types = types;
				_classification = null;
			}

			public TypeInfo Get() => _classification ?? (_classification = Refresh());

			TypeInfo Refresh()
			{
				if (_reader.HasAttributes)
				{
					if (_owner.Contains(_type))
					{
						return _type.Get(_owner);
					}

					if (_owner.Contains(_item))
					{
						return _item.Get(_owner);
					}

					if (_owner.Contains(_arguments))
					{
						var types = _arguments.Get(_owner);
						var generic = From(_generic).MakeGenericType(types.ToArray()).GetTypeInfo();
						return generic;
					}
				}
				return From(_types);
			}

			public void Clear() => _classification = null;

			TypeInfo From(ITypes types) => types.Get(_identities.Get(_owner.Name, _owner.Identifier));
		}
	}
}