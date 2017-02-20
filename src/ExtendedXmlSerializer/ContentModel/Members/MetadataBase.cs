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

namespace ExtendedXmlSerialization.ContentModel.Members
{
	abstract class MetadataBase<T> : MetadataBase, IMetadata<T> where T : MemberInfo
	{
		protected MetadataBase(T metadata, TypeInfo memberType) : base(metadata, memberType)
		{
			Metadata = metadata;
		}

		public new T Metadata { get; }
	}

	abstract class MetadataBase : IMetadata, IEquatable<IMetadata>
	{
		protected MetadataBase(MemberInfo metadata, TypeInfo memberType)
		{
			Metadata = metadata;
			MemberType = memberType;
		}

		public MemberInfo Metadata { get; }
		public TypeInfo MemberType { get; }

		public override bool Equals(object obj) => !ReferenceEquals(null, obj) &&
		                                           (ReferenceEquals(this, obj) || Equals((IMetadata) obj));

		public bool Equals(IMetadata other) => GetHashCode().Equals(other.GetHashCode());
		public override int GetHashCode() => Metadata.GetHashCode() ^ MemberType.GetHashCode();
	}
}