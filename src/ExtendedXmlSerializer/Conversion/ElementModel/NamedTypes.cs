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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Conversion.ElementModel
{
    public class NamedTypes : INamedTypes
    {
        public static NamedTypes Default { get; } = new NamedTypes();
        NamedTypes() : this(Defaults.Elements, Namespaces.Default) {}

        private readonly ImmutableArray<IElement> _elements;
        private readonly INamespaces _namespaces;

        public NamedTypes(IEnumerable<IElement> elements, INamespaces namespaces)
        {
            _elements = elements.ToImmutableArray();
            _namespaces = namespaces;
        }

        public TypeInfo Get(XName parameter)
        {
            var localName = parameter.LocalName;
            var ns = parameter.NamespaceName;
            var length = _elements.Length;
            for (int i = 0; i < length; i++)
            {
                var element = _elements[i];
                var info = element.OwnerType;
                if (ns == _namespaces.Get(info) && localName == element.Name)
                {
                    return info;
                }
            }
            return null;
        }
    }
}