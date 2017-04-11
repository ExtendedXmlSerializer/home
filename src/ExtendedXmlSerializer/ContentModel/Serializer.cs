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

using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel
{
	class Serializer : ISerializer
	{
		readonly IReader _reader;
		readonly IWriter _writer;

		public Serializer(IReader reader, IWriter writer)
		{
			_reader = reader;
			_writer = writer;
		}

		public void Write(IFormatWriter writer, object instance) => _writer.Write(writer, instance);
		public object Get(IFormatReader reader) => _reader.Get(reader);
	}

	class Serializer<T> : ISerializer<T>
	{
		readonly IReader<T> _reader;
		readonly IWriter<T> _writer;

		public Serializer(IReader<T> reader, IWriter<T> writer)
		{
			_reader = reader;
			_writer = writer;
		}

		public T Get(IFormatReader parameter) => _reader.Get(parameter);

		public void Write(IFormatWriter writer, T instance) => _writer.Write(writer, instance);
	}
}