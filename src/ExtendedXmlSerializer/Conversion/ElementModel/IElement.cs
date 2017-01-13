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

namespace ExtendedXmlSerialization.Conversion.ElementModel
{
    public interface IElement
    {
        IElementName Name { get; }
    }

    public interface IElementName
    {
        string DisplayName { get; }
        TypeInfo KeyedType { get; }
    }

    public class ElementName : IEquatable<ElementName>, IElementName
    {
        public ElementName(Type referencedType) : this(referencedType.GetTypeInfo()) {}
        public ElementName(TypeInfo referencedType) : this(referencedType, referencedType.Name) {}

        public ElementName(Type referencedType, string name) : this(referencedType.GetTypeInfo(), name) {}

        public ElementName(TypeInfo referencedType, string name)
        {
            KeyedType = referencedType;
            DisplayName = name;
        }

        public string DisplayName { get; }
        public TypeInfo KeyedType { get; }

        public bool Equals(ElementName other) => Equals(KeyedType, other.KeyedType);

        public override bool Equals(object obj) => 
            !ReferenceEquals(null, obj) && (obj is ElementName && Equals((ElementName) obj));

        public override int GetHashCode() => KeyedType?.GetHashCode() ?? 0;

        public static bool operator ==(ElementName left, ElementName right) => left.Equals(right);

        public static bool operator !=(ElementName left, ElementName right) => !left.Equals(right);
    }
}