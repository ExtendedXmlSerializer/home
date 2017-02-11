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

using System.Collections.Immutable;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.ContentModel.Xml.Namespacing
{
	class OptimizedXmlWriter : IXmlWriter
	{
		readonly static string Prefix = nameof(XNamespace.Xmlns).ToLower();
		readonly IXmlWriter _writer;
		readonly System.Xml.XmlWriter _native;
		readonly ImmutableArray<Namespace> _namespaces;

		public OptimizedXmlWriter(IXmlWriter writer, System.Xml.XmlWriter native, ImmutableArray<Namespace> namespaces)
		{
			_writer = writer;
			_native = native;
			_namespaces = namespaces;
		}

		public void Element(XName name)
		{
			var write = _native.WriteState;
			_writer.Element(name);

			switch (write)
			{
				case WriteState.Start:
					var length = _namespaces.Length;
					for (var i = 0; i < length; i++)
					{
						var ns = _namespaces[i];
						switch (i)
						{
							case 0:
								_native.WriteAttributeString(Prefix, ns.Identifier);
								break;
							default:
								_native.WriteAttributeString(Prefix, ns.Prefix, string.Empty, ns.Identifier);
								break;
						}
					}
					break;
			}
		}

		public string Get(TypeInfo parameter) => _writer.Get(parameter);

		public void Attribute(XName name, string value) => _writer.Attribute(name, value);

		public void Write(string text) => _writer.Write(text);

		public void EndCurrent() => _writer.EndCurrent();

		public void Member(string name) => _writer.Member(name);

		public void Dispose() => _writer.Dispose();
	}
}