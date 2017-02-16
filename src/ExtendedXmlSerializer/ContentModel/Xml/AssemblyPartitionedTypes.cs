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
	class AssemblyPartitionedTypes : ITypes
	{
		public static AssemblyPartitionedTypes Default { get; } = new AssemblyPartitionedTypes();

		AssemblyPartitionedTypes()
			: this(AssemblyPathParser.Default, AssemblyLoader.Default, TypePartitions.Default, TypeNameAlteration.Default) {}

		readonly IParseContext<AssemblyPath> _parser;
		readonly IAssemblyLoader _loader;
		readonly ITypePartitions _partitions;
		readonly IAlteration<string> _names;

		public AssemblyPartitionedTypes(IParseContext<AssemblyPath> parser, IAssemblyLoader loader, ITypePartitions partitions,
		                                IAlteration<string> names)
		{
			_parser = parser;
			_loader = loader;
			_partitions = partitions;
			_names = names;
		}

		public TypeInfo Get(IIdentity parameter)
		{
			var parse = _parser.Get().TryParse(parameter.Identifier);
			if (parse.WasSuccessful)
			{
				var assembly = _loader.Get(parse.Value.Path);

				var result =
					assembly.GetType($"{parse.Value.Namespace}.{_names.Get(parameter.Name)}", false, false)?.GetTypeInfo() ??
					_partitions.Get(assembly)?.Invoke(parse.Value.Namespace)?.Invoke(parameter.Name);
				return result;
			}
			return null;
		}
	}
}