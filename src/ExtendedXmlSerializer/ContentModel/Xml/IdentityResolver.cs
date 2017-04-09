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

using System.Xml.Linq;

namespace ExtendedXmlSerializer.ContentModel.Xml
{
	sealed class IdentityResolver : IIdentityResolver
	{
		readonly static string Xmlns = XNamespace.Xmlns.NamespaceName;


		readonly IIdentityResolver _resolver;
		readonly System.Xml.XmlWriter _writer;

		public IdentityResolver(System.Xml.XmlWriter writer, IIdentityResolver resolver)
		{
			_writer = writer;
			_resolver = resolver;
		}

		public string Get(string identifier) => _writer.LookupPrefix(identifier ?? string.Empty) ?? Create(identifier);

		string Create(string identifier)
		{
			var prefix = _resolver.Get(identifier);
			_writer.WriteAttributeString(prefix, Xmlns, identifier);
			return _writer.LookupPrefix(identifier);
		}
	}
}