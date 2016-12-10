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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerialization.Extensibility;
using ExtendedXmlSerialization.ProcessModel.Write;
using ExtendedXmlSerialization.Services;

namespace ExtendedXmlSerialization
{
    public class SerializationToolsFactoryHost : ServiceRepository, IExtension, ISerializationToolsFactoryHost
    {
        private readonly Func<IWritingContext> _context;
        private readonly IExtension _extension;

        public SerializationToolsFactoryHost(IImmutableList<object> services)
            : this(() => new DefaultWritingContext(), services) {}

        public SerializationToolsFactoryHost(Func<IWritingContext> context,
                                             IImmutableList<object> services)
            : this(context, new OrderedSet<IExtension>(services.OfType<IExtension>()), services) {}

        public SerializationToolsFactoryHost(Func<IWritingContext> context, IList<IExtension> extensions,
                                             IEnumerable<object> services) : this(context, extensions, new CompositeExtension(extensions), services)
        {}

        SerializationToolsFactoryHost(Func<IWritingContext> context, IList<IExtension> extensions, IExtension extension,
                                             IEnumerable<object> services) : base(services)
        {
            _context = context;
            _extension = extension;
            Extensions = extensions;
        }

        ISerializationToolsFactory Factory { get; set; }

        public IWritingContext New() => _context();

        public void Assign(ISerializationToolsFactory factory) => Factory = factory;

        public IList<IExtension> Extensions { get; }

        public IExtendedXmlSerializerConfig GetConfiguration(Type type) => Factory?.GetConfiguration(type);

        public IPropertyEncryption EncryptionAlgorithm => Factory?.EncryptionAlgorithm;
        public bool Starting(IServiceProvider services) => _extension.Starting(services);

        public void Finished(IServiceProvider services) => _extension.Finished(services);
    }
}