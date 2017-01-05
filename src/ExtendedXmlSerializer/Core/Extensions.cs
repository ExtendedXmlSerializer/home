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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Core
{
    public static class Extensions
    {
        public static IEnumerable<T> Append<T>(this T @this, params T[] second) => @this.Append(second.AsEnumerable());

        public static IEnumerable<T> Append<T>(this T @this, ImmutableArray<T> second)
            => @this.Append_(second.ToArray());

        public static IEnumerable<T> Append<T>(this T @this, IEnumerable<T> second) => @this.Append_(second);

        static IEnumerable<T> Append_<T>(this T @this, IEnumerable<T> second)
        {
            yield return @this;
            foreach (var element1 in second)
                yield return element1;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> @this, params T[] items) => @this.Concat(items);

        public static IEnumerable<T> Append<T>(this IEnumerable<T> @this, T element)
        {
            foreach (var element1 in @this)
                yield return element1;
            yield return element;
        }

        public static IEnumerable<T> Yield<T>(this T @this)
        {
            yield return @this;
        }

        public static string NullIfEmpty(this string target) => string.IsNullOrEmpty(target) ? null : target;

        // ATTRIBUTION: http://stackoverflow.com/a/5461399/3602057
        public static bool IsAssignableFromGeneric(this Type @this, Type candidate)
        {
            var interfaceTypes = candidate.GetInterfaces();

            foreach (var it in interfaceTypes.Append(candidate))
            {
                if (it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == @this)
                    return true;
            }

            var baseType = candidate.GetTypeInfo().BaseType;
            var result = baseType != null && IsAssignableFromGeneric(@this, baseType);
            return result;
        }

        public static TResult Accept<TParameter, TResult>(this TResult @this, TParameter _) => @this;

        public static ISpecification<object> Adapt<T>(this ISpecification<T> @this)
            => new SpecificationAdapter<T>(@this);

        public static Type GetMemberType(this MemberInfo memberInfo) =>
            (memberInfo as MethodInfo)?.ReturnType ??
            (memberInfo as PropertyInfo)?.PropertyType ??
            (memberInfo as FieldInfo)?.FieldType ??
            (memberInfo as TypeInfo)?.AsType();

        public static bool IsWritable(this MemberInfo memberInfo) =>
            (memberInfo as PropertyInfo)?.CanWrite ??
            !(memberInfo as FieldInfo)?.IsInitOnly ?? false;


        public static T To<T>(this object @this) => @this is T ? (T) @this : default(T);

        public static T Get<T>(this IServiceProvider @this)
            => @this is T ? (T) @this : @this.GetService(typeof(T)).To<T>();

        public static T GetValid<T>(this IServiceProvider @this)
            => @this is T ? (T) @this : @this.GetService(typeof(T)).AsValid<T>();

        public static T AsValid<T>(this object @this, string message = null)
        {
            if (@this is T)
            {
                return (T) @this;
            }

            throw new InvalidOperationException(message ??
                                                $"'{@this.GetType().FullName}' is not of type {typeof(T).FullName}.");
        }
    }
}