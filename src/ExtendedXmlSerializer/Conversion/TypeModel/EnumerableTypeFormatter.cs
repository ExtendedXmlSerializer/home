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

using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerialization.Conversion.TypeModel
{
    class EnumerableTypeFormatter : ITypeFormatter
    {
        public static EnumerableTypeFormatter Default { get; } = new EnumerableTypeFormatter();
        EnumerableTypeFormatter() : this(TypeFormatter.Default, CollectionItemTypeLocator.Default) {}

        private readonly ITypeFormatter _formatter;
        private readonly ICollectionItemTypeLocator _locator;

        public EnumerableTypeFormatter(ITypeFormatter formatter, ICollectionItemTypeLocator locator)
        {
            _formatter = formatter;
            _locator = locator;
        }

        public string Format(TypeInfo type)
        {
            var arguments = type.GetGenericArguments();
            var name = arguments.Any()
                ? string.Join(string.Empty, arguments.Select(p => p.Name))
                : _formatter.Format(_locator.Get(type));
            var result = $"{type.Name.ToStringArray('`')[0]}Of{name}";
            return result;
        }
    }
}