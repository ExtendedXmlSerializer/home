// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using System.Linq.Expressions;

namespace ExtendedXmlSerialization.Cache
{
    internal static class ObjectAccessors
    {
        internal delegate object ObjectActivator();

        internal delegate object PropertyGetter(object item);

        internal delegate void PropertySetter(object item, object value);

        internal static ObjectActivator CreateObjectActivator(Type type)
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                return null;

            NewExpression newExp = Expression.New(type);

            var lambda = Expression.Lambda<ObjectActivator>(newExp);

            return lambda.Compile();
        }
        
        internal static PropertyGetter CreatePropertyGetter(Type type, string propertyName)
        {
            // Object (type object) from witch the data are retrieved
            ParameterExpression itemObject = Expression.Parameter(typeof(object), "item");

            // Object casted to specific type using the operator "as".
            UnaryExpression itemCasted = Expression.TypeAs(itemObject, type);

            // Property from casted object
            MemberExpression property = Expression.Property(itemCasted, propertyName);

            // Because we use this function also for value type we need to add conversion to object
            Expression conversion = Expression.Convert(property, typeof(object));

            LambdaExpression lambda = Expression.Lambda(typeof(PropertyGetter), conversion, itemObject);

            PropertyGetter compiled = (PropertyGetter)lambda.Compile();
            return compiled;
        }

        internal static PropertySetter CreatePropertySetter(Type type, string propertyName)
        {
            // Object (type object) from witch the data are retrieved
            ParameterExpression itemObject = Expression.Parameter(typeof(object), "item");

            // Object casted to specific type using the operator "as".
            UnaryExpression itemCasted = Expression.TypeAs(itemObject, type);

            // Property from casted object
            MemberExpression property = Expression.Property(itemCasted, propertyName);

            // Secound parameter - value to set
            ParameterExpression value = Expression.Parameter(typeof(object), "value");

            // Because we use this function also for value type we need to add conversion to object
            Expression paramCasted = Expression.Convert(value, property.Type);

            // Assign value to property
            BinaryExpression assign = Expression.Assign(property, paramCasted);

            LambdaExpression lambda = Expression.Lambda(typeof(PropertySetter), assign, itemObject, value);

            PropertySetter compiled = (PropertySetter)lambda.Compile();
            return compiled;
        }
    }
}
