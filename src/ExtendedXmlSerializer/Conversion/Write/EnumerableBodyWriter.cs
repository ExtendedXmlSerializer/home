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
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Conversion.Write
{
	public class EnumerableBodyWriter : EnumerableBodyWriter<IEnumerable>
	{
		public EnumerableBodyWriter(IWriter item) : base(item) {}
	}

	public class EnumerableBodyWriter<T> : WriterBase<T> where T : class, IEnumerable
	{
		readonly IWriter _item;

		public EnumerableBodyWriter(IWriter item)
		{
			_item = item;
		}

		protected virtual IEnumerator Get(T instance) => instance.GetEnumerator();

		protected override void Write(IWriteContext context, T instance)
		{
			var element = (ICollectionElement) context.Element;
			var item = context.New(element.Element, element.Element.Classification);
			var enumerator = Get(instance);
			while (enumerator.MoveNext())
			{
				using (item.Emit())
				{
					_item.Write(item, enumerator.Current);
				}
			}
		}
	}
}