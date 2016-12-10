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
using System.IO;
using ExtendedXmlSerialization.Extensibility;
using ExtendedXmlSerialization.ProcessModel.Write;

namespace ExtendedXmlSerialization
{
    class Serialization : ISerialization
    {
        private readonly ISerializationToolsFactoryHost _host;
        private readonly ISerializer _serializer;

        public Serialization(ISerializationToolsFactoryHost host, ISerializer serializer)
        {
            _host = host;
            _serializer = serializer;
        }

        public void Serialize(Stream stream, object instance) => _serializer.Serialize(stream, instance);
        public IList<IExtension> Extensions => _host.Extensions;
        public IExtendedXmlSerializerConfig GetConfiguration(Type type) => _host.GetConfiguration(type);
        public IPropertyEncryption EncryptionAlgorithm => _host.EncryptionAlgorithm;
        public IWritingContext New() => _host.New();
        public void Assign(ISerializationToolsFactory factory) => _host.Assign(factory);
        public object GetService(Type serviceType) => _host.GetService(serviceType);
        public void Add(object service) => _host.Add(service);
    }
}