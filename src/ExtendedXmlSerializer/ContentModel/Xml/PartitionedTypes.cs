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
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	class PartitionedTypes : IPartitionedTypes
	{
		readonly static Func<TypeInfo, bool> Specification = CanPartitionSpecification.Default.IsSatisfiedBy;

		public static PartitionedTypes Default { get; } = new PartitionedTypes();
		PartitionedTypes() : this(PartitionFormatter.Default.Get) {}

		readonly Func<TypeInfo, bool> _specification;
		readonly Func<TypeInfo, string> _group;

		public PartitionedTypes(Func<TypeInfo, string> group) : this(Specification, group) {}

		public PartitionedTypes(Func<TypeInfo, bool> specification, Func<TypeInfo, string> group)
		{
			_specification = specification;
			_group = group;
		}

		public ILookup<string, TypeInfo> Get(Assembly parameter)
			=> parameter.ExportedTypes.YieldMetadata(_specification).ToLookup(_group);
	}
}