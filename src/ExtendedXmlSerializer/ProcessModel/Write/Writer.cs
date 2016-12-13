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
using System.Xml;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Elements;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    public class Writer : IWriter
    {
        private readonly XmlWriter _writer;

        public Writer(XmlWriter writer)
        {
            _writer = writer;
        }

        public void Begin(string elementName, Uri identifier = null)
        {
            var id = identifier?.ToString();
            switch (_writer.WriteState)
            {
                case WriteState.Start:
                    _writer.WriteStartDocument();
                    break;
            }

            _writer.WriteStartElement(elementName, id);
        }

        public void EndElement() => _writer.WriteEndElement();
        public void Emit(string text) => _writer.WriteString(text);
        public void Emit(string attribute, string value, Uri identifier = null, string prefix = null)
            => _writer.WriteAttributeString(prefix, attribute, identifier?.ToString(), value);

        public void Emit(string attribute, Uri identity, string name, string value)
        {
            _writer.WriteStartAttribute(attribute, identity.ToString());
            _writer.WriteQualifiedName(name, value);
            _writer.WriteEndAttribute();
        }

        public void Dispose()
        {
            _writer.WriteEndDocument();
            _writer.Dispose();
        }
    }
}