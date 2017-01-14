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
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Write
{
    public class EnumerableBodyWriter : WriterBase<IEnumerable>
    {
        private readonly IElementNameSelector _names;
        private readonly IWriter _item;
        private readonly ICollectionItemTypeLocator _locator;

        public EnumerableBodyWriter(IWriter item) : this(ElementNames.Default, item) {}

        public EnumerableBodyWriter(IElementNameSelector names, IWriter item) : this(names, item, CollectionItemTypeLocator.Default) {}

        public EnumerableBodyWriter(IElementNameSelector names, IWriter item, ICollectionItemTypeLocator locator)
        {
            _names = names;
            _item = item;
            _locator = locator;
        }

        protected override void Write(IWriteContext context, IEnumerable instance)
        {
            var elementType = _locator.Get(instance.GetType().GetTypeInfo());
            foreach (var item in instance)
            {
                var name = _names.Get(item.GetType().GetTypeInfo());
                using (var child = context.Start(new CollectionItem(name, elementType)))
                {
                    _item.Write(child, item);
                }
            }
        }
    }
}