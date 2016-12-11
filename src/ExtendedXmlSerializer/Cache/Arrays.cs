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
using System.Collections;
using System.Linq;
using ExtendedXmlSerialization.Plans.Write;
using ExtendedXmlSerialization.Specifications;

namespace ExtendedXmlSerialization.Cache
{
    class Arrays : WeakCache<object, Array>
    {
        private readonly ISpecification<Type> _specification;
        readonly static Array Array = (object[]) Enumerable.Empty<object>();

        public static Arrays Default { get; } = new Arrays();
        private Arrays() : this(IsEnumerableTypeSpecification.Default) {}

        Arrays(ISpecification<Type> specification) : base(key => null)
        {
            _specification = specification;
        }

        public Array AsArray(object instance)
        {
            var items = Get(instance);
            if (items != null)
            {
                return items;
            }
            var result = Is(instance)
                ? (instance as Array ?? ((IEnumerable) instance).Cast<object>().ToArray())
                : Array;
            Default.Add(instance, result);
            return result;
        }

        public bool Is(object instance) => _specification.IsSatisfiedBy(instance.GetType());
    }
}