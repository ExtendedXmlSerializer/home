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
using System.Reflection;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	public struct MemberDescriptor : IEquatable<MemberDescriptor>
	{
		readonly int _code;

		public MemberDescriptor(TypeInfo reflectedType, MemberInfo metadata, TypeInfo memberType, bool writable = true)
			: this(reflectedType, metadata, memberType, writable, (metadata.GetHashCode() * 397) ^ memberType.GetHashCode()) {}

		MemberDescriptor(TypeInfo reflectedType, MemberInfo metadata, TypeInfo memberType, bool writable, int code)
		{
			_code = code;
			ReflectedType = reflectedType;
			Metadata = metadata;
			MemberType = memberType;
			Writable = writable;
		}

		public TypeInfo ReflectedType { get; }
		public MemberInfo Metadata { get; }
		public TypeInfo MemberType { get; }
		public bool Writable { get; }

		public bool Equals(MemberDescriptor other)
		{
			var code = _code;
			return code.Equals(other._code);
		}

		public override bool Equals(object obj)
			=> !ReferenceEquals(null, obj) && obj is MemberDescriptor && Equals((MemberDescriptor) obj);

		public override int GetHashCode() => _code;

		public static bool operator ==(MemberDescriptor left, MemberDescriptor right) => left.Equals(right);

		public static bool operator !=(MemberDescriptor left, MemberDescriptor right) => !left.Equals(right);
	}
}