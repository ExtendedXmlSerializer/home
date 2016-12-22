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

using System.IO;
using System.Xml;
using ExtendedXmlSerialization.Configuration.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
    public class SimpleSerializer : ISerializer
    {
        public static SimpleSerializer Default { get; } = new SimpleSerializer();
        SimpleSerializer() {}

        public void Serialize(Stream stream, object instance)
        {
            using (var writer = new LegacyWriter(XmlWriter.Create(stream)))
            {
                var selector = new MutableEntitySelector();
                selector.Selector = new EntitySelector(new EntityBuilder(selector));
                var serialization = new Serialization(new RootBuilder(selector.Selector), new DefaultEmitter(writer));
                serialization.Execute(instance);
            }
        }
    }

    public class LegacySerializer : ISerializer
    {
        private readonly ISerializationToolsFactory _tools;
        private readonly IIdentityLocator _locator;
        private readonly IEncryptionFactory _encryption;

        public LegacySerializer(ISerializationToolsFactory tools)
            : this(tools, new IdentityLocator(tools.Locate)) {}

        public LegacySerializer(ISerializationToolsFactory tools, IIdentityLocator locator)
            : this(tools, locator, new EncryptionFactory(tools)) {}

        public LegacySerializer(ISerializationToolsFactory tools, IIdentityLocator locator,
                                                   IEncryptionFactory encryption)
        {
            _tools = tools;
            _locator = locator;
            _encryption = encryption;
        }

        public void Serialize(Stream stream, object instance)
        {
            var monitor = new ContextMonitor();
            var xmlWriter = XmlWriter.Create(stream);
            var inner = new LegacyWriter(xmlWriter, new EncryptedObjectSerializer(monitor, _encryption));
            using (var writer = new LegacyCustomWriter(_tools, inner, xmlWriter))
            {
                var selector = new MutableEntitySelector();
                selector.Selector = new EntitySelector(new EntityBuilder(selector));
                var serialization = new Serialization(new RootBuilder(selector.Selector),
                                                      new LegacyEmitter(writer, monitor, _locator));
                serialization.Execute(instance);
            }
        }
    }
}