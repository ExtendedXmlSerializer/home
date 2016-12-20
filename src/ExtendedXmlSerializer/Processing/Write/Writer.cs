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
using ExtendedXmlSerialization.Configuration.Write;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Model.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
    public class Writer : IWriter
    {
        private readonly XmlWriter _writer;
        private readonly IObjectSerializer _serializer;
        private readonly INamespaceLocator _locator;
        private readonly IDisposable _end;

        public Writer(XmlWriter writer) : this(writer, ObjectSerializer.Default, DefaultNamespaceLocator.Default) {}

        public Writer(XmlWriter writer, IObjectSerializer serializer, INamespaceLocator locator)
        {
            _writer = writer;
            _serializer = serializer;
            _locator = locator;
            _end = new DelegatedDisposable(End);
        }

        private void End() => _writer.WriteEndElement();

        public void Emit(object instance) => _writer.WriteString(_serializer.Serialize(instance));

        public IDisposable Begin(IQualifiedNode definition)
        {
            var identifier = _locator.Locate(definition.DeclaredType);
            var id = identifier?.ToString();
            switch (_writer.WriteState)
            {
                case WriteState.Start:
                    _writer.WriteStartDocument();
                    break;
            }

            _writer.WriteStartElement(definition.Name, id);
            return _end;
        }

        public void Emit(IObjectNode node)
        {
            var instance = node.Instance;
            var identifier = _locator.Locate(node.DeclaredType)?.ToString();
            var text = _serializer.Serialize(instance);
            var type = instance as Type;
            if (identifier != null && type != null)
            {
                var identity = _locator.Locate(type)?.ToString();
                if (identity != null)
                {
                    _writer.WriteStartAttribute(node.Name, identifier);
                    var name = TypeDefinitions.Default.Get(type).Name;
                    _writer.WriteQualifiedName(name, identity);
                    _writer.WriteEndAttribute();
                    return;
                }
            }
            _writer.WriteAttributeString(node.Name, identifier, text);
        }

        public void Dispose()
        {
            _writer.WriteEndDocument();
            _writer.Dispose();
        }
    }
}