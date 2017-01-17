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
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Conversion
{
    public class Selector : WeakCacheBase<TypeInfo, IConverter>, ISelector
    {
        private readonly IElementSelector _elements;
        private readonly IConverterSelector _converters;

        public Selector(IElementSelector elements, IConverterSelector converters)
        {
            _elements = elements;
            _converters = converters;
        }

        protected override IConverter Create(TypeInfo parameter)
        {
            var element = _elements.Get(parameter);
            var result = _converters.Get(element);
            return result;
        }

        public IConverter Get(IElement parameter) => _converters.Get(parameter);
        //public IConverter Get(TypeInfo parameter) => _elements.Get(parameter);
    }
}