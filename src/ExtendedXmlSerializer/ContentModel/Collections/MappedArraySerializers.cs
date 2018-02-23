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

using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class MappedArraySerializers : IContents
	{
		readonly IContents _contents;

		public MappedArraySerializers(IContents contents)
		{
			_contents = contents;
		}


		public ISerializer Get(TypeInfo parameter)
		{
			var serializer = _contents.Get(parameter);
			var result = new Serializer(new Reader(serializer, parameter), serializer);
			return result;
		}

		sealed class Reader : IReader
		{
			readonly IReader _reader;
			readonly IProperty<ImmutableArray<int>> _map;
			readonly TypeInfo _root;

			public Reader(IReader reader, TypeInfo type)
				: this(reader, MapProperty.Default, RootType.Default.Get(type))
			{
			}

			public Reader(IReader reader, IProperty<ImmutableArray<int>> map, TypeInfo root)
			{
				_reader = reader;
				_map = map;
				_root = root;
			}


			public object Get(IFormatReader parameter)
			{
				var dimensions = _map.Get(parameter);
				var source = _reader.Get(parameter).AsValid<Array>();
				var result = Array.CreateInstance(_root, dimensions.ToArray());
				Buffer.BlockCopy(source, 0, result, 0, source.Length * sizeof(int));
				return result;
			}
		}
	}
}