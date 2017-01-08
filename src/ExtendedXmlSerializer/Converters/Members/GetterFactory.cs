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
using System.Reflection;

namespace ExtendedXmlSerialization.Converters.Members
{
    public class GetterFactory : IGetterFactory
    {
        public static GetterFactory Default { get; } = new GetterFactory();
        GetterFactory() {}

        public Func<object, object> Get(MemberInfo parameter) => Get(parameter.DeclaringType, parameter.Name);

        static Func<object, object> Get(Type type, string name)
        {
            // Object (type object) from witch the data are retrieved
            var itemObject = Expression.Parameter(typeof(object), "item");

            // Object casted to specific type using the operator "as".
            var itemCasted = Expression.Convert(itemObject, type);

            // Property from casted object
            var property = Expression.PropertyOrField(itemCasted, name);

            // Because we use this function also for value type we need to add conversion to object
            var conversion = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<object, object>>(conversion, itemObject);
            var result = lambda.Compile();
            return result;
        }
    }
}