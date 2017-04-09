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
using ExtendedXmlSerializer.ContentModel.Xml;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class DelegatedSerializer : DelegatedSerializer<object>, ISerializer
	{
		public DelegatedSerializer(Func<string, object> deserialize, Func<object, string> serialize)
			: base(deserialize, serialize) {}
	}

	class DelegatedSerializer<T> : ISerializer<T>
	{
		readonly Func<string, T> _deserialize;
		readonly Func<T, string> _serialize;

		public DelegatedSerializer(Func<string, T> deserialize, Func<T, string> serialize)
		{
			_deserialize = deserialize;
			_serialize = serialize;
		}

		public void Write(IXmlWriter writer, T instance) => writer.Write(_serialize(instance));

		public T Get(IReader reader) => _deserialize(reader.Content());
	}
}