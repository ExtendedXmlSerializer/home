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
using ExtendedXmlSerializer.ContentModel.Xml;

namespace ExtendedXmlSerializer.ContentModel
{
	class Identity : IIdentity, IEquatable<IIdentity>
	{
		readonly static IdentityComparer IdentityComparer = IdentityComparer.Default;
		readonly int _code;

		public Identity(string name, string identifier)
		{
			Name = name;
			Identifier = identifier;
			_code = IdentityComparer.GetHashCode(this);
		}

		public string Name { get; }
		public string Identifier { get; }

		public static bool operator ==(Identity left, Identity right) => left._code == right._code;
		public static bool operator !=(Identity left, Identity right) => left._code != right._code;
		public bool Equals(IIdentity other) => _code == other.GetHashCode();
		public sealed override int GetHashCode() => _code;
		public sealed override bool Equals(object obj) => obj is IIdentity && Equals((IIdentity) obj);
	}
}