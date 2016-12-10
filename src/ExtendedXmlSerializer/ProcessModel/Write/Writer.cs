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
        private readonly IObjectSerializer _serializer;
        private readonly INamespaceLocator _locator;
        private readonly INamespaceEmitter _emitter;
        private readonly XmlWriter _writer;

        public Writer(IObjectSerializer serializer, INamespaceLocator locator, INamespaceEmitter emitter,
                      XmlWriter writer)
        {
            _serializer = serializer;
            _locator = locator;
            _emitter = emitter;
            _writer = writer;
        }

        public void Start(IRootElement root)
        {
            _writer.WriteStartDocument();
            Begin(root);
            var identifier = root.Identifier?.ToString();
            if (identifier != null)
            {
                _writer.WriteAttributeString("xmlns", identifier);
                _emitter.Execute(root.Root);
            }
        }

        public void Begin(IElement element) => _writer.WriteStartElement(element.Name, element.Identifier?.ToString());

        public void EndElement() => _writer.WriteEndElement();
        public void Emit(object instance) => _writer.WriteString(_serializer.Serialize(instance));

        public void Emit(IProperty property)
        {
            var ns = property.Identifier?.ToString();
            var type = property.Value as Type;
            if (ns != null && type != null)
            {
                var identifier = _locator.Get(property.Value);
                if (identifier != null)
                {
                    var name = TypeDefinitionCache.GetDefinition(type).Name;
                    _writer.WriteStartAttribute(property.Name, ns);
                    _writer.WriteQualifiedName(name, identifier.ToString());
                    _writer.WriteEndAttribute();
                    return;
                }
            }
            var serialized = _serializer.Serialize(property.Value);
            _writer.WriteAttributeString(property.Prefix, property.Name, ns, serialized);
        }

        public void Dispose()
        {
            _writer.WriteEndDocument();
            _writer.Dispose();
        }
    }
}