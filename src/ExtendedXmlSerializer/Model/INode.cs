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
using ExtendedXmlSerialization.Processing;

namespace ExtendedXmlSerialization.Model
{
    /*public interface IIdentity
    {
        string Name { get; }

        Type Key { get; }
    }

    public abstract class Identity<T> : Identity
    {
        readonly private static Type DefaultScope = typeof(T);

        protected Identity() : base(DefaultScope) {}

        protected Identity(string name) : this(DefaultScope, name) {}

        protected Identity(Type key, string name) : base(key, name) {}
    }

    public class LocalIdentity : Identity
    {
        public LocalIdentity(string name) : base(null, name) {}
    }

    public class Identity : IIdentity
    {
        public Identity(Type key) : this(key, key.Name) {}

        public Identity(Type key, string name)
        {
            Key = key;
            Name = name;
        }

        public string Name { get; }

        public Type Key { get; }
    }*/

    public static class Extensions
    {
        readonly private static Func<Type, ITypeDefinition> Definition = TypeDefinitions.Default.Get;

        public static ITypeDefinition For(this ITypeDefinition @this, object value)
        {
            var type = value?.GetType();
            var result = type != null && type != @this.Type ? Definition(type) : @this;
            return result;
        }

        /*public static object Instance(this IInstance @this)
            => (@this as IPrimitive)?.Value ?? (@this as IObject)?.Instance;*/
    }
}