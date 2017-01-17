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

using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion
{
    public class SelectingConverter : ConverterBase
    {
        private readonly ISelector _selector;

        public SelectingConverter(ISelector selector)
        {
            _selector = selector;
        }

        public override void Write(IWriteContext context, object instance)
        {
            var converter = _selector.Get(context.Element) ?? _selector.Get(instance.GetType().GetTypeInfo());
            converter.Write(context, instance);
        }

        public override object Read(IReadContext context)
        {
            var container = (context as IReadContainerContext).Container;
            if (container != null)
            {
                // HACK: Fix this.
                var converter = _selector.Get(container) ?? _selector.Get(context.Element);
                var ctx = new XmlReadContext(XmlReadContextFactory.Default, null, context.Element,
                                             context.Get<XElement>(), context.DisplayName);
                var result = converter.Read(ctx);
                return result;
            }
            else
            {
                var converter = _selector.Get(context.Element);
                var result = converter.Read(context);
                return result;
            }
            
        }
    }
}