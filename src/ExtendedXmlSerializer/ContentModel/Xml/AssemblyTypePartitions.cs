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
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	class AssemblyTypePartitions : ReferenceCacheBase<Assembly, Partition>, ITypePartitions
	{
		readonly static Func<TypeInfo, string> Formatter = TypeFormatter.Default.Get;
		readonly static ApplicationTypes ApplicationTypes = ApplicationTypes.Default;

		public static AssemblyTypePartitions Default { get; } = new AssemblyTypePartitions();
		AssemblyTypePartitions() : this(HasAliasSpecification.Default) {}

		readonly IApplicationTypes _types;
		readonly Func<TypeInfo, bool> _specification;
		readonly Func<TypeInfo, string> _key;

		public AssemblyTypePartitions(ISpecification<TypeInfo> specification)
			: this(ApplicationTypes, specification.IsSatisfiedBy, x => x.Namespace) {}

		public AssemblyTypePartitions(IApplicationTypes types, Func<TypeInfo, bool> specification, Func<TypeInfo, string> key)
		{
			_types = types;
			_specification = specification;
			_key = key;
		}

		protected override Partition Create(Assembly parameter) =>
			_types.Get(parameter)
			      .Where(_specification)
			      .ToLookup(_key)
			      .ToDictionary(x => x.Key, x => new Func<string, TypeInfo>(x.ToDictionary(Formatter).Get))
			      .Get;

		public TypeInfo Get(TypePartition parameter)
		{
			var partition = base.Get(parameter.Assembly);
			var ns = partition?.Invoke(parameter.Namespace);
			var result = ns?.Invoke(parameter.Name);
			return result;
		}
	}
}