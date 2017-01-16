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
using System.Reflection;

namespace ExtendedXmlSerialization.ElementModel
{
    public class ElementName : IEquatable<ElementName>, IElementName
    {
        public ElementName(Type classification) : this(classification.GetTypeInfo()) {}
        public ElementName(TypeInfo classification) : this(classification, classification.Name) {}

        public ElementName(Type classification, string name) : this(classification.GetTypeInfo(), name) {}

        public ElementName(TypeInfo classification, string name)
        {
            Classification = classification;
            DisplayName = name;
        }

        public string DisplayName { get; }
        public TypeInfo Classification { get; }

        public bool Equals(ElementName other) => Equals(Classification, other.Classification);

        public override bool Equals(object obj) =>
            !ReferenceEquals(null, obj) && (obj is ElementName && Equals((ElementName) obj));

        public override int GetHashCode() => Classification?.GetHashCode() ?? 0;

        public static bool operator ==(ElementName left, ElementName right) => left.Equals(right);

        public static bool operator !=(ElementName left, ElementName right) => !left.Equals(right);
    }
}