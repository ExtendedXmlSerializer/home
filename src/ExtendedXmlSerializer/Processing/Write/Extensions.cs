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
using System.IO;
using ExtendedXmlSerialization.Model;
using ExtendedXmlSerialization.Model.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
    public static class Extensions
    {
        readonly private static Func<Type, ITypeDefinition> Definition = TypeDefinitions.Default.Get;

        public static ITypeDefinition For(this ITypeDefinition @this, object value)
        {
            var type = value?.GetType();
            if (type != null && type != @this.Type)
            {
                return Definition(type);
            }
            return @this;
        }

        public static object Locate(this ISerializationToolsFactory @this, object arg)
            => @this?.GetConfiguration(arg.GetType())?.GetObjectId(arg);

        public static object Instance(this IContext @this) => @this.Entity.Instance();
        public static object Instance(this IEntity @this) => (@this as IPrimitive)?.Value ?? (@this as IObject)?.Instance;

        public static string Serialize(this ISerializer @this, object instance)
        {
            using (var stream = new MemoryStream())
            {
                @this.Serialize(stream, instance);
                stream.Seek(0, SeekOrigin.Begin);

                var result = new StreamReader(stream).ReadToEnd();
                return result;
            }
        }

        // public static object Value(this IMemberScope @this) => @this.Instance.GetValue(@this.Parent.Instance);

        // public static IDisposable NewInstance<T>(this ISerialization @this, T instance, IElementInformation information) => @this.New(instance, information);

        /*public static void Attach(this ISerialization @this, IProperty property) => 
            AttachedProperties.Default.Attach(@this.Current.Instance, property);

        public static IImmutableList<IProperty> GetProperties(this ISerialization @this)
        {
            var list = AttachedProperties.Default.GetProperties(@this.Current.Instance);
            var result = list.ToImmutableList();
            list.Clear();
            return result;
        }*/
    }
}