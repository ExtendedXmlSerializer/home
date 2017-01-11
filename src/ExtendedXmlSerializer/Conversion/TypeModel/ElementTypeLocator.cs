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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.TypeModel
{
    public class ElementTypeLocator : WeakCacheBase<Type, Type>, IElementTypeLocator
    {
        readonly static TypeInfo ArrayInfo = typeof(Array).GetTypeInfo();
        public static ElementTypeLocator Default { get; } = new ElementTypeLocator();
        ElementTypeLocator() {}

        // Attribution: http://stackoverflow.com/a/17713382/3602057
        protected override Type Create(Type parameter)
        {
            // Type is Array
            // short-circuit if you expect lots of arrays 
            if (ArrayInfo.IsAssignableFrom(parameter))
                return parameter.GetElementType();

            // type is IEnumerable<T>;
            if (parameter.GetTypeInfo().IsGenericType && parameter.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return parameter.GetGenericArguments()[0];

            // type implements/extends IEnumerable<T>;
            var result = parameter.GetInterfaces()
                                  .Where(t => t.GetTypeInfo().IsGenericType &&
                                              t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                                  .Select(t => t.GenericTypeArguments[0]).FirstOrDefault();

            return result;
        }
    }
}