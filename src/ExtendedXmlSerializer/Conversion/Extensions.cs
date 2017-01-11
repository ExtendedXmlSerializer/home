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
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.TypeModel;

namespace ExtendedXmlSerialization.Conversion
{
    public static class Extensions
    {
        public static T Annotated<T>(this XElement @this, T item)
        {
            @this.AddAnnotation(item);
            return item;
        }

        public static XElement Initialized(this IElementTypes @this, XElement element, Typing typing)
        {
            if (typing != null && @this.Get(element) == null)
            {
                element.Annotated(typing);
            }
            return element;
        }


        public static TypeInfo AccountForNullable(this TypeInfo @this)
            => Nullable.GetUnderlyingType(@this.AsType())?.GetTypeInfo() ?? @this;

        public static Type AccountForNullable(this Type @this) => Nullable.GetUnderlyingType(@this) ?? @this;

        public static T Activate<T>(this IActivators @this, Typing type) => (T) @this.Get(type).Invoke();
    }
}