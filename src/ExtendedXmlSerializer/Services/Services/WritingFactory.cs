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
using System.IO;
using System.Xml;
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Extensibility;
using ExtendedXmlSerialization.Extensibility.Write;

namespace ExtendedXmlSerialization.Services.Services
{
    public class WritingFactory : IWritingFactory
    {
        private readonly INamespaceLocator _locator;
        private readonly INamespaces _namespaces;
        private readonly ISerializationToolsFactory _tools;
        private readonly IServiceProvider _services;
        private readonly IExtension _extension;
        private readonly Func<IWritingContext> _context;

        public WritingFactory(
            INamespaceLocator locator,
            INamespaces namespaces,
            ISerializationToolsFactory tools,
            IServiceProvider services,
            Func<IWritingContext> context, IExtension extension)
        {
            _locator = locator;
            _namespaces = namespaces;
            _tools = tools;
            _services = services;
            _extension = extension;
            _context = context;
        }

        public IWriting Get(Stream parameter)
        {
            var context = _context();
            var settings = new XmlWriterSettings {NamespaceHandling = NamespaceHandling.OmitDuplicates, Indent = true};
            var xmlWriter = XmlWriter.Create(parameter, settings);
            var serializer = new EncryptedObjectSerializer(new EncryptionSpecification(_tools, context), _tools);
            var writer = new Writer(serializer, _locator, new NamespaceEmitter(xmlWriter, _namespaces), xmlWriter);
            var result = new Writing(writer, context, _locator
                                     /*services:*/, serializer, _extension, _tools, _services, this, parameter, context,
                                     settings, xmlWriter, serializer, writer);
            return result;
        }
    }
}