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

using System.Collections;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class ArrayReader : IReader
	{
		readonly IClassification _classification;
		readonly TypeInfo _declaredType;
		readonly IReader<ArrayList> _reader;

		public ArrayReader(IInnerContentServices services, IClassification classification, TypeInfo elementType, IReader item)
			: this(classification,
					elementType,
					services.CreateContents<ArrayList>(new ConditionalInnerContentHandler(services, new CollectionInnerContentHandler(item, services))))
		{
		}

		ArrayReader(IClassification classification, TypeInfo elementType, IReader<ArrayList> reader)
		{
			_classification = classification;
			_declaredType = elementType;
			_reader = reader;
		}

		public object Get(IFormatReader parameter)
		{
			var elementType = _classification.GetClassification(parameter, _declaredType).GetElementType();
			var result = _reader.Get(parameter).ToArray(elementType);
			return result;
		}
	}
}