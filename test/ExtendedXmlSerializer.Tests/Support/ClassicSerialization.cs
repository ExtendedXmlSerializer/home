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

using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer.Tests.Support
{
	sealed class ClassicSerialization<T>
	{
		public static ClassicSerialization<T> Default { get; } = new ClassicSerialization<T>();
		ClassicSerialization() : this(new XmlSerializer(typeof(T)), new XmlReaderFactory()) {}

		readonly XmlSerializer _serializer;
		readonly IXmlReaderFactory _factory;

		public ClassicSerialization(XmlSerializer serializer, IXmlReaderFactory factory)
		{
			_serializer = serializer;
			_factory = factory;
		}

		public T Get(string data)

		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
			{
				using (var reader = _factory.Get(stream))
				{
					var result = (T) _serializer.Deserialize(reader);
					return result;
				}
			}
		}

		public string Get(T instance)
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = XmlWriter.Create(stream))
				{
					_serializer.Serialize(writer, instance);
					writer.Flush();
					stream.Seek(0, SeekOrigin.Begin);
					var result = new StreamReader(stream).ReadToEnd();
					return result;
				}
			}
		}
	}
}