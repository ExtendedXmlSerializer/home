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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion.Formatting;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class AssemblyTypePartitions : CacheBase<Assembly, Partition>, IAssemblyTypePartitions
	{
		readonly static IApplicationTypes ApplicationTypes = TypeModel.ApplicationTypes.All;

		readonly IApplicationTypes _types;
		readonly Func<TypeInfo, bool> _specification;
		readonly Func<TypeInfo, string> _formatter;
		readonly Func<TypeInfo, string> _key;
		readonly Func<IGrouping<string, TypeInfo>, Func<string, ImmutableArray<TypeInfo>?>> _format;

		public AssemblyTypePartitions(IPartitionedTypeSpecification specification, ITypeFormatter formatter)
			: this(specification, formatter.Get) {}

		public AssemblyTypePartitions(ISpecification<TypeInfo> specification, Func<TypeInfo, string> formatter)
			: this(specification.IsSatisfiedBy, formatter, ApplicationTypes, x => x.Namespace) {}

		public AssemblyTypePartitions(Func<TypeInfo, bool> specification, Func<TypeInfo, string> formatter,
		                              IApplicationTypes types, Func<TypeInfo, string> key)
		{
			_types = types;
			_specification = specification;
			_formatter = formatter;
			_key = key;
			_format = Format;
		}

		protected override Partition Create(Assembly parameter)
			=> _types.Get(parameter)
			         .Where(_specification)
			         .ToLookup(_key)
			         .Where(x => !string.IsNullOrWhiteSpace(x.Key))
			         .ToDictionary(x => x.Key, _format)
			         .Get;

		public ImmutableArray<TypeInfo>? Get(TypePartition parameter)
			=> Get(parameter.Assembly).Invoke(parameter.Namespace)?.Invoke(parameter.Name);

		Func<string, ImmutableArray<TypeInfo>?> Format(IGrouping<string, TypeInfo> grouping)
			=> grouping.ToLookup(_formatter)
			           .ToDictionary(x => x.Key, x => x.ToImmutableArray())
			           .GetStructure;
	}
}