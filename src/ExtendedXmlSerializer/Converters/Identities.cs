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
using System.Linq;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Converters
{
    public class Identities : IIdentities
    {
        public static Identities Default { get; } = new Identities();
        Identities() : this(string.Empty) {}

        private readonly IDictionary<Type, XName> _names;
        private readonly IDictionary<XName, Type> _types;

        public Identities(string namespaceName)
            : this(new Dictionary<Type, XName>
                   {
                       {typeof(bool), XName.Get("boolean", namespaceName)},
                       {typeof(char), XName.Get("char", namespaceName)},
                       {typeof(sbyte), XName.Get("byte", namespaceName)},
                       {typeof(byte), XName.Get("unsignedByte", namespaceName)},
                       {typeof(short), XName.Get("short", namespaceName)},
                       {typeof(ushort), XName.Get("unsignedShort", namespaceName)},
                       {typeof(int), XName.Get("int", namespaceName)},
                       {typeof(uint), XName.Get("unsignedInt", namespaceName)},
                       {typeof(long), XName.Get("long", namespaceName)},
                       {typeof(ulong), XName.Get("unsignedLong", namespaceName)},
                       {typeof(float), XName.Get("float", namespaceName)},
                       {typeof(double), XName.Get("double", namespaceName)},
                       {typeof(decimal), XName.Get("decimal", namespaceName)},
                       {typeof(DateTime), XName.Get("dateTime", namespaceName)},
                       {typeof(DateTimeOffset), XName.Get("dateTimeOffset", namespaceName)},
                       {typeof(string), XName.Get("string", namespaceName)},
                       {typeof(Guid), XName.Get("guid", namespaceName)},
                       {typeof(TimeSpan), XName.Get("TimeSpan", namespaceName)}
                   }) {}

        public Identities(IDictionary<Type, XName> names)
            : this(names, names.ToDictionary(x => x.Value, y => y.Key)) {}

        public Identities(IDictionary<Type, XName> names, IDictionary<XName, Type> types)
        {
            _names = names;
            _types = types;
        }

        public XName Get(Type parameter)
        {
            XName result;
            return _names.TryGetValue(parameter, out result) ? result : null;
        }

        public Type Get(XName parameter)
        {
            Type result;
            return _types.TryGetValue(parameter, out result) ? result : null;
        }

        public bool IsSatisfiedBy(Type parameter) => _names.ContainsKey(parameter);

        public bool IsSatisfiedBy(XName parameter) => _types.ContainsKey(parameter);
    }
}