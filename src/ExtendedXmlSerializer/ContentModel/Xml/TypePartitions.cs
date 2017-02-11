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
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	class TypePartitions : ITypePartitions
	{
		readonly static Func<TypeInfo, string> Formatter = TypeFormatter.Default.Get;
		readonly static Func<TypeInfo, bool> Specification = CanPartitionSpecification.Default.IsSatisfiedBy;

		readonly Func<TypeInfo, bool> _specification;
		readonly Func<TypeInfo, string> _key;

		public static TypePartitions Default { get; } = new TypePartitions();
		TypePartitions() : this(x => x.Namespace) {}

		public TypePartitions(Func<TypeInfo, string> key) : this(Specification, key) {}

		public TypePartitions(Func<TypeInfo, bool> specification, Func<TypeInfo, string> key)
		{
			_specification = specification;
			_key = key;
		}

		public Partition Get(Assembly parameter) =>
			parameter.ExportedTypes.YieldMetadata(_specification)
			         .ToLookup(_key)
			         .ToDictionary(x => x.Key, x => new Func<string, TypeInfo>(x.ToDictionary(Formatter).Get))
			         .Get;
	}
}