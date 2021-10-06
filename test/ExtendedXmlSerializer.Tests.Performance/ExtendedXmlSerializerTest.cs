// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
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
using System.Text;
using System.Xml;
using BenchmarkDotNet.Attributes;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Performance.Model;

namespace ExtendedXmlSerializer.Tests.Performance
{
    [ShortRunJob]
    [MemoryDiagnoser]
    public class ExtendedXmlSerializerTest
    {
        readonly IExtendedXmlSerializer _serializer = new ConfigurationContainer().EnableClassicMode().Create();

        readonly TestClassOtherClass _obj = new TestClassOtherClass().Init();
        readonly byte[] _data;

        readonly IXmlReaderFactory _readerFactory = new XmlReaderFactory();

        public ExtendedXmlSerializerTest()
        {
            _data = Encoding.UTF8.GetBytes(SerializationClassWithPrimitive());
            DeserializationClassWithPrimitive();
        }

        [Benchmark]
        public string SerializationClassWithPrimitive()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream))
                {
                    _serializer.Serialize(writer, _obj);
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    var result = new StreamReader(stream).ReadToEnd();
                    return result;
                }
            }
        }

        [Benchmark]
        public object DeserializationClassWithPrimitive()
        {
            using (var stream = new MemoryStream(_data))
            {
                using (var reader = _readerFactory.Get(stream))
                {
                    return _serializer.Deserialize(reader);
                }
            }
        }
    }
}