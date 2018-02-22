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
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class MappedArrayElement : IElements
	{
		readonly IElements _element;

		public MappedArrayElement(IElements element)
		{
			_element = element;
		}

		public IWriter Get(TypeInfo parameter)
		{
			var writer = _element.Get(parameter);
			var result = new Writer(writer, Dimensions.Defaults.Get(parameter)).Adapt();
			return result;
		}

		sealed class Writer : IWriter<Array>
		{
			readonly IWriter _writer;
			readonly IProperty<ImmutableArray<int>> _property;
			readonly Func<Array, ImmutableArray<int>> _dimensions;

			public Writer(IWriter writer, Func<Array, ImmutableArray<int>> dimensions) : this(writer, MapProperty.Default,
				dimensions)
			{
			}

			public Writer(IWriter writer, IProperty<ImmutableArray<int>> property, Func<Array, ImmutableArray<int>> dimensions)
			{
				_writer = writer;
				_property = property;
				_dimensions = dimensions;
			}

			public void Write(IFormatWriter writer, Array instance)
			{
				_writer.Write(writer, instance);
				_property.Write(writer, _dimensions(instance));
			}
		}
	}
}