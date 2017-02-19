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
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;
using Sprache;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	class PartitionedTypes : ITypes
	{
		public static PartitionedTypes Default { get; } = new PartitionedTypes();
		PartitionedTypes() : this(TypeLoader.Default, AssemblyTypePartitions.Default) {}

		public PartitionedTypes(params ITypePartitions[] partitions)
			: this(AssemblyPathParser.Default, AssemblyLoader.Default, new TypePartitions(partitions)) {}

		readonly IParseContext<AssemblyPath> _parser;
		readonly IAssemblyLoader _loader;
		readonly ITypePartitions _partitions;

		public PartitionedTypes(IParseContext<AssemblyPath> parser, IAssemblyLoader loader, ITypePartitions partitions)
		{
			_parser = parser;
			_loader = loader;
			_partitions = partitions;
		}

		public TypeInfo Get(IIdentity parameter)
		{
			var parse = _parser.Get().TryParse(parameter.Identifier);
			if (parse.WasSuccessful)
			{
				var path = parse.Value;
				var partition = new TypePartition(_loader.Get(path.Path), path.Namespace, parameter.Name);
				var result = _partitions.Get(partition);
				return result;
			}
			return null;
		}
	}
}