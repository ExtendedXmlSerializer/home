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
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Extensibility.Write;
using ExtendedXmlSerialization.NodeModel.Write;
using ExtendedXmlSerialization.Services;

namespace ExtendedXmlSerialization.ProcessModel.Write
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

        public IDisposable Begin(IObjectNode instance)
        {
            var identifier = _locator.Locate(instance.Instance);
            var id = identifier?.ToString();
            switch (_writer.WriteState)
            {
                case WriteState.Start:
                    _writer.WriteStartDocument();
                    break;
            }

            _writer.WriteStartElement(instance.Name, id);
            return _end;
        }

        private void End() => _writer.WriteEndElement();

        public void Emit(IObjectNode node) => _writer.WriteString(_serializer.Serialize(node.Instance));

        /*var property = node as IProperty;
            if (property != null)
            {
                var instance = property.Instance;
                var identifier = _locator.Locate(instance) ?? _locator.Locate(property.Type);
                var text = _serializer.Serialize(instance);
                _writer.WriteAttributeString(node.Name, identifier?.ToString(), text);
                return;
            }*/

        /*
        public void Emit(IProperty property)
        {
            var type = property.Value as Type;
            if (property.Identifier != null && type != null)
            {
                var identifier = _locator.Locate(type)?.ToString();
                if (identifier != null)
                {
                    var name = TypeDefinitionCache.GetDefinition(type).Name;
                    _writer.Emit(property.Name, property.Identifier, name, identifier);
                    return;
                }
            }
            var serialized = _serializer.Serialize(property.Value);
            _writer.Emit(property.Name, serialized, property.Identifier, property.Prefix);
        }

        public void Emit(Type type) {}*/

        /*
        public void Emit(string attribute, Uri identity, string name, string value)
        {
            _writer.WriteStartAttribute(attribute, identity.ToString());
            _writer.WriteQualifiedName(name, value);
            _writer.WriteEndAttribute();
        }*/

        public void Dispose()
        {
            _writer.WriteEndDocument();
            _writer.Dispose();
        }
    }
}