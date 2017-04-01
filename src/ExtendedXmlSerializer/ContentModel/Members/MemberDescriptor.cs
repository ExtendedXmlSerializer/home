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
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	public struct MemberDescriptor : IEquatable<MemberDescriptor>
	{
		public static implicit operator MemberDescriptor(MemberInfo member) => MemberDescriptors.Default.Get(member);

		readonly int _code;

		public MemberDescriptor(PropertyInfo metadata)
			: this(metadata, metadata.PropertyType.GetTypeInfo(), metadata.CanWrite) {}

		public MemberDescriptor(PropertyInfo metadata, TypeInfo memberType)
			: this(metadata, Validate(metadata, metadata.PropertyType.GetTypeInfo(), memberType), metadata.CanWrite) {}

		public MemberDescriptor(FieldInfo metadata)
			: this(metadata, metadata.FieldType.GetTypeInfo(), !metadata.IsInitOnly) {}

		[UsedImplicitly]
		public MemberDescriptor(FieldInfo metadata, TypeInfo memberType)
			: this(metadata, Validate(metadata, metadata.FieldType.GetTypeInfo(), memberType), !metadata.IsInitOnly) {}

		MemberDescriptor(MemberInfo metadata, TypeInfo memberType, bool writable)
			: this(metadata, memberType, writable, (metadata.GetHashCode() * 397) ^ memberType.GetHashCode()) {}

		MemberDescriptor(MemberInfo metadata, TypeInfo memberType, bool writable, int code)
		{
			_code = code;
			Metadata = metadata;
			MemberType = memberType;
			Writable = writable;
		}

		static TypeInfo Validate(MemberInfo member, TypeInfo defaultType, TypeInfo assigned)
		{
			if (!Equals(defaultType, assigned) && !defaultType.IsAssignableFrom(assigned))
			{
				throw new InvalidOperationException(
					$"Cannot assign type '{assigned}' as a return type for '{member}'.  Ensure that the specified type can be assigned from '{defaultType}'");
			}
			return assigned;
		}

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