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

namespace ExtendedXmlSerialization.ExtensionModel
{
	public struct ReferenceIdentity : IEquatable<ReferenceIdentity>
	{
		readonly int _code;

		public ReferenceIdentity(TypeInfo type, object identifier)
			: this((type.GetHashCode() * 397) ^ identifier.GetHashCode()) {}

		public ReferenceIdentity(TypeInfo type, uint identifier) : this((type.GetHashCode() * 397) ^ identifier.GetHashCode()) {}

		ReferenceIdentity(int code)
		{
			_code = code;
		}

		public bool Equals(ReferenceIdentity other)
		{
			var code = _code;
			var result = code.Equals(other._code);
			return result;
		}

		public override bool Equals(object obj)
			=> !ReferenceEquals(null, obj) && obj is ReferenceIdentity && Equals((ReferenceIdentity) obj);

		public override int GetHashCode() => _code;

		public static bool operator ==(ReferenceIdentity left, ReferenceIdentity right) => left.Equals(right);

		public static bool operator !=(ReferenceIdentity left, ReferenceIdentity right) => !left.Equals(right);
	}
}