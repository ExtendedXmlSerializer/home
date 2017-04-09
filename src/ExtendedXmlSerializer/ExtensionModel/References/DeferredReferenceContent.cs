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
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.ExtensionModel.Content;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredReferenceContent : ISerializer
	{
		readonly IReservedItems _reserved;
		readonly IRootReferences _references;
		readonly ISerializer _serializer;

		public DeferredReferenceContent(IRootReferences references, ISerializer serializer)
			: this(ReservedItems.Default, references, serializer) {}

		public DeferredReferenceContent(IReservedItems reserved, IRootReferences references, ISerializer serializer)
		{
			_reserved = reserved;
			_references = references;
			_serializer = serializer;
		}

		public object Get(IFormatReader parameter) => _serializer.Get(parameter);

		public void Write(IXmlWriter writer, object instance)
		{
			var list = instance as IList;
			if (list != null)
			{
				var references = _references.Get(writer);
				var hold = _reserved.Get(writer);
				foreach (var item in list)
				{
					if (references.Contains(item))
					{
						hold.Get(item).Push(list);
					}
				}
			}

			_serializer.Write(writer, instance);
		}
	}
}