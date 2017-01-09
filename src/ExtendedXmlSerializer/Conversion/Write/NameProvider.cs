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

using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Write
{
    public class NameProvider : NameProviderBase
    {
        public static NameProvider Default { get; } = new NameProvider();
        NameProvider() : this(TypeNameFormatter.Default, string.Empty) {}

        private readonly ITypeFormatter _formatter;
        private readonly string _defaultNamespace;

        public NameProvider(ITypeFormatter formatter, string defaultNamespace)
        {
            _formatter = formatter;
            _defaultNamespace = defaultNamespace;
        }

        protected override XName Create(TypeInfo type, MemberInfo member)
        {
            var attribute = type.GetCustomAttribute<XmlRootAttribute>(false);
            var name = attribute?.ElementName.NullIfEmpty() ?? _formatter.Format(new Typed(type));
            var ns = attribute?.Namespace ?? _defaultNamespace;
            var result = XName.Get(name, ns);
            return result;
        }
    }
}