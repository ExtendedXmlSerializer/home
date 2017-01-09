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

using ExtendedXmlSerialization.Conversion.Legacy;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion
{
    public class RootConverters<T> : WeakCacheBase<T, IConverter>, IRootConverters<T> where T : class
    {
        private readonly ITypes _types;
        private readonly ISelectorFactory _selector;

        public RootConverters(ITypes types, ISelectorFactory selector)
        {
            _types = types;
            _selector = selector;
        }

        protected override IConverter Create(T parameter)
        {
            var source = new AssignableSelector();
            var selector = new Converter(new SelectingReader(_types, source), new SelectingWriter(source));

            source.Execute(_selector.Get(selector));

            return selector;
        }
    }

    public class RootConverters : RootConverters<object>
    {
        public static RootConverters Default { get; } = new RootConverters();
        RootConverters() : base(Types.Default, SelectorFactory.Default) {}
    }
}