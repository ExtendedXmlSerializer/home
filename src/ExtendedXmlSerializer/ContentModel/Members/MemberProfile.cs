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
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	public struct MemberProfile
	{
		public MemberProfile(
			ISpecification<object> specification,
			string name,
			bool allowWrite,
			int order,
			MemberInfo metadata,
			TypeInfo memberType,
			ISerializer content,
			IReader reader,
			IWriter writer)
			: this(
				specification, new Identity(name, string.Empty), allowWrite, order, metadata, memberType, content, reader, writer) {}

		public MemberProfile(
			ISpecification<object> specification,
			IIdentity identity,
			bool allowWrite,
			int order,
			MemberInfo metadata,
			TypeInfo memberType,
			ISerializer content,
			IReader reader,
			IWriter writer)

		{
			Specification = specification;
			Identity = identity;
			AllowWrite = allowWrite;
			Order = order;
			Metadata = metadata;
			MemberType = memberType;
			Content = content;
			Writer = writer;
			Reader = reader;
		}

		public ISpecification<object> Specification { get; }

		public IIdentity Identity { get; }

		public bool AllowWrite { get; }

		public int Order { get; }

		public MemberInfo Metadata { get; }
		public TypeInfo MemberType { get; }

		public ISerializer Content { get; }
		public IReader Reader { get; }
		public IWriter Writer { get; }
	}
}