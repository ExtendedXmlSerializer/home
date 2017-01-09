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
using System.Linq.Expressions;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.TypeModel
{
    class AddDelegates : WeakCacheBase<Type, Action<object, object>>, IAddDelegates
    {
        public static AddDelegates Default { get; } = new AddDelegates();
        AddDelegates() : this(ElementTypeLocator.Default, AddMethodLocator.Default) {}

        private readonly IElementTypeLocator _locator;
        private readonly IAddMethodLocator _add;

        public AddDelegates(IElementTypeLocator locator, IAddMethodLocator add)
        {
            _locator = locator;
            _add = add;
        }

        protected override Action<object, object> Create(Type parameter)
        {
            var elementType = _locator.Locate(parameter);
            if (elementType != null)
            {
                var add = _add.Locate(parameter, elementType);
                if (add != null)
                {
                    // Object (type object) from witch the data are retrieved
                    var itemObject = Expression.Parameter(typeof(object), "item");
                    var value = Expression.Parameter(typeof(object), "value");

                    // Object casted to specific type using the operator "as".
                    var itemCasted = Expression.Convert(itemObject, parameter);

                    var castedParam = Expression.Convert(value, elementType);

                    var conversion = Expression.Call(itemCasted, add, castedParam);

                    var lambda = Expression.Lambda<Action<object, object>>(conversion, itemObject, value);

                    var result = lambda.Compile();
                    return result;
                }
            }

            return null;
        }
    }
}