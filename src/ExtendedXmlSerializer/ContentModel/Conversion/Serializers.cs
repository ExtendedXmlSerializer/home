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

using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class Serializers : CacheBase<TypeInfo, ISerializer>, ISerializers
	{
		readonly IConverters _converters;
		readonly IContentReaders _readers;
		readonly IContentWriters _writers;
		readonly Content.ISerializers _fallback;

		public Serializers(IConverters converters, IContentReaders readers, IContentWriters writers,
		                   Content.ISerializers fallback)
		{
			_converters = converters;
			_readers = readers;
			_writers = writers;
			_fallback = fallback;
		}

		protected override ISerializer Create(TypeInfo parameter)
		{
			var converter = _converters.Get(parameter);
			var result = converter != null
				             ? new Serializer(_readers.Get(converter.Parse), _writers.Get(converter.Format))
				             : _fallback.Get(parameter);
			return result;
		}
	}

	sealed class Serializers<T> : ISerializers<T>
	{
		readonly IConverters<T> _converters;
		readonly Content.IContentReaders<T> _readers;
		readonly Content.IContentWriters<T> _writers;
		readonly IContents<T> _fallback;

		public Serializers(IConverters<T> converters, Content.IContentReaders<T> readers, Content.IContentWriters<T> writers,
		                   DeferredContents<T> fallback)
		{
			_converters = converters;
			_readers = readers;
			_writers = writers;
			_fallback = fallback;
		}

		public IContentSerializer<T> Get()
		{
			var converter = _converters.Get();
			var result = converter != null
				             ? new ContentSerializer<T>(_readers.Get(converter.Parse), _writers.Get(converter.Format))
				             : _fallback.Get();
			return result;
		}
	}
}