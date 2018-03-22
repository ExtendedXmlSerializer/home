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
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	public class Collections : IContents
	{
		readonly ISerializers               _serializers;
		readonly ICollectionItemTypeLocator _locator;
		readonly ICollectionContents        _contents;

		public Collections(RuntimeSerializers serializers, ICollectionContents contents)
			: this(serializers, CollectionItemTypeLocator.Default, contents) {}

		public Collections(RuntimeSerializers serializers, ICollectionItemTypeLocator locator, ICollectionContents contents)
		{
			_serializers = serializers;
			_locator     = locator;
			_contents    = contents;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var itemType   = _locator.Get(parameter);
			var serializer = _serializers.Get(itemType);
			var item       = new Serializer(serializer, new CollectionItemAwareWriter(serializer));
			var result     = _contents.Get(new CollectionContentInput(item, parameter, itemType));
			return result;
		}
	}
}