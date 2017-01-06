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
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Processing
{
    public struct AddDelegateParameter
    {
        public AddDelegateParameter(Type type, Type elementType)
        {
            Type = type;
            ElementType = elementType;
        }

        public Type Type { get; }
        public Type ElementType { get; }
    }

    public interface IAddDelegateFactory : IParameterizedSource<AddDelegateParameter, Action<object, object>> {}

    class AddDelegateFactory : IAddDelegateFactory
    {
        public static AddDelegateFactory Default { get; } = new AddDelegateFactory();
        AddDelegateFactory() : this(AddMethodLocator.Default) {}

        private readonly IAddMethodLocator _locator;

        public AddDelegateFactory(IAddMethodLocator locator)
        {
            _locator = locator;
        }

        public Action<object, object> Get(AddDelegateParameter parameter)
        {
            var add = _locator.Locate(parameter.Type, parameter.ElementType);
            // Object (type object) from witch the data are retrieved
            var itemObject = Expression.Parameter(typeof(object), "item");

            // Object casted to specific type using the operator "as".
            var itemCasted = Expression.Convert(itemObject, parameter.Type);

            var value = Expression.Parameter(typeof(object), "value");

            var castedParam = Expression.Convert(value, parameter.ElementType);

            var conversion = Expression.Call(itemCasted, add, castedParam);

            var lambda = Expression.Lambda<Action<object, object>>(conversion, itemObject, value);

            var result = lambda.Compile();
            return result;
        }
    }


    public interface IActivators : IParameterizedSource<Type, Func<object>> {}

    class Activators : WeakCacheBase<Type, Func<object>>, IActivators
    {
        public static Activators Default { get; } = new Activators();
        Activators() {}

        protected override Func<object> Create(Type parameter)
        {
            var newExp = Expression.Convert(Expression.New(parameter), typeof(object));
            var lambda = Expression.Lambda<Func<object>>(newExp);
            var result = lambda.Compile();
            return result;
        }
    }
}