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

using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Sprache;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ContentModel.Xml
{
	sealed class PartitionedTypeCandidates : StructureCacheBase<IIdentity, ImmutableArray<TypeInfo>>, ITypeCandidates
	{
		readonly static AssemblyPathParser AssemblyPathParser = AssemblyPathParser.Default;
		readonly static AssemblyLoader AssemblyLoader = AssemblyLoader.Default;

		public PartitionedTypeCandidates(params ITypePartitions[] partitions)
			: this(AssemblyPathParser, AssemblyLoader, new TypePartitions(partitions)) { }

		readonly Parser<AssemblyPath> _parser;
		readonly IAssemblyLoader _loader;
		readonly ITypePartitions _partitions;

		public PartitionedTypeCandidates(Parser<AssemblyPath> parser, IAssemblyLoader loader, ITypePartitions partitions)
		{
			_parser = parser;
			_loader = loader;
			_partitions = partitions;
		}

		protected override ImmutableArray<TypeInfo> Create(IIdentity parameter)
		{
			var parse = _parser.TryParse(parameter.Identifier);
			if (parse.WasSuccessful)
			{
				var path = parse.Value;
				var partition = new TypePartition(_loader.Get(path.Path), path.Namespace, parameter.Name);
				var array = _partitions.Get(partition);
				var result = array ?? ImmutableArray<TypeInfo>.Empty;
				return result;
			}
			return ImmutableArray<TypeInfo>.Empty;
		}
	}
}