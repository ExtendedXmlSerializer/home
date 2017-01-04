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

namespace ExtendedXmlSerialization.Processing
{
    internal static class ObjectAccessors
    {
        internal delegate object ObjectActivator();

        internal delegate object PropertyGetter(object item);

        internal delegate void PropertySetter(object item, object value);

        internal delegate void AddItemToCollection(object item, object value);

        internal static ObjectActivator CreateObjectActivator(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            //if isClass or struct but not abstract, enum or primitive
            if (!typeInfo.IsAbstract &&
                (typeInfo.IsValueType || typeInfo.IsClass && type.GetConstructor(Type.EmptyTypes) != null))
                //class must have constructor
            {
                var newExp = Expression.Convert(Expression.New(type), typeof(object));

                var lambda = Expression.Lambda<ObjectActivator>(newExp);

                return lambda.Compile();
            }

            return null;
        }

        internal static PropertyGetter CreatePropertyGetter(MemberInfo member)
        {
            if (member.DeclaringType != null)
            {
                // Object (type object) from witch the data are retrieved
                ParameterExpression itemObject = Expression.Parameter(typeof(object), "item");

                // Object casted to specific type using the operator "as".
                UnaryExpression itemCasted = Expression.Convert(itemObject, member.DeclaringType);

                // Property from casted object
                MemberExpression property = Expression.PropertyOrField(itemCasted, member.Name);

                // Because we use this function also for value type we need to add conversion to object
                Expression conversion = Expression.Convert(property, typeof(object));

                LambdaExpression lambda = Expression.Lambda(typeof(PropertyGetter), conversion, itemObject);

                PropertyGetter compiled = (PropertyGetter) lambda.Compile();
                return compiled;
            }
            return null;
        }

        internal static AddItemToCollection CreateMethodAddCollection(Type type, Type parameterType, MethodInfo add)
        {
            // Object (type object) from witch the data are retrieved
            ParameterExpression itemObject = Expression.Parameter(typeof(object), "item");

            // Object casted to specific type using the operator "as".
            UnaryExpression itemCasted = Expression.Convert(itemObject, type);

            /*var parameterType = elementType ?? type.GetGenericArguments()[0];*/

            ParameterExpression value = Expression.Parameter(typeof(object), "value");

            Expression castedParam = Expression.Convert(value, parameterType);

            Expression conversion = Expression.Call(itemCasted, add, castedParam);

            LambdaExpression lambda = Expression.Lambda(typeof(AddItemToCollection), conversion, itemObject, value);

            AddItemToCollection compiled = (AddItemToCollection) lambda.Compile();
            return compiled;
        }

        internal static PropertySetter CreatePropertySetter(Type type, string propertyName)
        {
            // Object (type object) from witch the data are retrieved
            ParameterExpression itemObject = Expression.Parameter(typeof(object), "item");

            // Object casted to specific type using the operator "as".
            Expression itemCasted = type.GetTypeInfo().IsValueType
                ? Expression.Unbox(itemObject, type)
                : Expression.Convert(itemObject, type);
            // Property from casted object
            MemberExpression property = Expression.PropertyOrField(itemCasted, propertyName);

            // Secound parameter - value to set
            ParameterExpression value = Expression.Parameter(typeof(object), "value");

            // Because we use this function also for value type we need to add conversion to object
            Expression paramCasted = Expression.Convert(value, property.Type);

            // Assign value to property
            BinaryExpression assign = Expression.Assign(property, paramCasted);

            LambdaExpression lambda = Expression.Lambda(typeof(PropertySetter), assign, itemObject, value);

            PropertySetter compiled = (PropertySetter) lambda.Compile();
            return compiled;
        }
    }
}