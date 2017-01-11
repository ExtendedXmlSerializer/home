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

using ExtendedXmlSerialization.Conversion.TypeModel;

namespace ExtendedXmlSerialization.Conversion.ElementModel
{
    public class Element : IElement
    {
        public Element(Typing ownerType) : this(ownerType, ownerType.Type.Name) {}

        public Element(Typing ownerType, string name)
        {
            ReferencedType = ownerType;
            Name = name;
        }

        public string Name { get; }
        public Typing ReferencedType { get; }

        /*public bool Equals(Element other) =>
            !ReferenceEquals(null, other) &&
            (ReferenceEquals(this, other) ||
             string.Equals(Name, other.Name) && Equals(OwnerType, other.OwnerType));

        public override bool Equals(object obj) =>
            !ReferenceEquals(null, obj) &&
            (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Element) obj));

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name?.GetHashCode() ?? 0) * 397) ^
                       (OwnerType?.GetHashCode() ?? 0);
            }
        }

        public static bool operator ==(Element left, Element right) => Equals(left, right);

        public static bool operator !=(Element left, Element right) => !Equals(left, right);*/
    }
}