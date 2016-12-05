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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

/*using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;*/

namespace ExtendedXmlSerialization.Common
{
    public interface ITypeFormatter
    {
        string Format(Type type);
    }

    class DefaultTypeFormatter : ITypeFormatter
    {
        public static DefaultTypeFormatter Default { get; } = new DefaultTypeFormatter();
        DefaultTypeFormatter() {}

        public string Format(Type type) => type.FullName;
    }

    /// <summary>
    /// https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/ReflectionUtils.cs
    /// </summary>
    class TypeFormatter : ITypeFormatter
    {
        public static TypeFormatter Default { get; } = new TypeFormatter();
        TypeFormatter() : this(FormatterAssemblyStyle.Simple, new DefaultSerializationBinder()) {}

        private readonly FormatterAssemblyStyle _style;
        private readonly SerializationBinder _binder;

        public TypeFormatter(FormatterAssemblyStyle style, SerializationBinder binder)
        {
            _style = style;
            _binder = binder;
        }

        public string Format(Type type)
        {
            var name = FullyQualifiedTypeName(type);
            switch (_style)
            {
                case FormatterAssemblyStyle.Simple:
                    return RemoveAssemblyDetails(name);
                default:
                    return name;
            }
        }

        private static string RemoveAssemblyDetails(string fullyQualifiedTypeName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag1 = false;
            bool flag2 = false;
            for (int index = 0; index < fullyQualifiedTypeName.Length; ++index)
            {
                char ch = fullyQualifiedTypeName[index];
                switch (ch)
                {
                    case ',':
                        if (!flag1)
                        {
                            flag1 = true;
                            stringBuilder.Append(ch);
                            break;
                        }
                        flag2 = true;
                        break;
                    case '[':
                        flag1 = false;
                        flag2 = false;
                        stringBuilder.Append(ch);
                        break;
                    case ']':
                        flag1 = false;
                        flag2 = false;
                        stringBuilder.Append(ch);
                        break;
                    default:
                        if (!flag2)
                        {
                            stringBuilder.Append(ch);
                        }
                        break;
                }
            }
            return stringBuilder.ToString();
        }


        private string FullyQualifiedTypeName(Type type)
        {
            string typeName, assemblyName;
            _binder.BindToName(type, out assemblyName, out typeName);
            var result = $"{typeName}{(assemblyName != null ? $", {assemblyName}" : string.Empty)}";
            return result;
        }
    }

    /*class PackUriFormatter : ITypeFormatter
    {
        public string Format(Type type)
        {
            return string.Format("clr-namespace:{0};assembly={1}");
        }
    }*/
}