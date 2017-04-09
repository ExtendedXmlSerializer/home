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
using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.ExtensionModel.Content;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredReferenceSerializer : ISerializer
	{
		readonly IReservedItems _reserved;
		readonly ISerializer _serializer;

		public DeferredReferenceSerializer(ISerializer serializer) : this(ReservedItems.Default, serializer) {}

		public DeferredReferenceSerializer(IReservedItems reserved, ISerializer serializer)
		{
			_reserved = reserved;
			_serializer = serializer;
		}

		public object Get(IFormatReader parameter) => _serializer.Get(parameter);

		public void Write(IXmlWriter writer, object instance)
		{
			var lists = _reserved.Get(writer);
			foreach (var o in Yield(instance))
			{
				var reserved = lists.Get(o);
				if (reserved.Any())
				{
					reserved.Pop();
				}
			}

			_serializer.Write(writer, instance);
		}

		static IEnumerable<object> Yield(object instance)
		{
			if (instance is DictionaryEntry)
			{
				var entry = (DictionaryEntry) instance;
				yield return entry.Key;
				yield return entry.Value;
			}
			else
			{
				yield return instance;
			}
		}
	}
}