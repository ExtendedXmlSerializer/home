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
using System.Reflection;
using System.Xml;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	public interface IXmlReader : IIdentity, IPrefixAware, IDisposable
	{
		TypeInfo Classification { get; }

		string Value();

		IXmlAttributes Attributes { get; }

		IXmlContent Content { get; }
	}

	public interface IXmlContent : IIdentity
	{
		int New();

		void Reset();

		bool Next(int depth);
	}

	class XmlContent : IXmlContent
	{
		readonly System.Xml.XmlReader _reader;
		public XmlContent(System.Xml.XmlReader reader)
		{
			_reader = reader;
		}

		public string Identifier => _reader.Prefix;
		public string Name => _reader.Prefix;

		public int New()
		{
			if (_reader.HasAttributes && _reader.NodeType == XmlNodeType.Attribute)
			{
				Reset();
			}
			return _reader.Depth + 1;
		}

		public void Reset() => _reader.MoveToElement();

		public bool Next(int depth) => _reader.Read() && _reader.IsStartElement() && _reader.Depth == depth;
	}
}