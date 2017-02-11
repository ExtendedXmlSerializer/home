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
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	class TypeMaps : CacheBase<XNamespace, ITypeMap>, ITypes // TODO: Move to weak reference (currently this is faster).
	{
		public static TypeMaps Default { get; } = new TypeMaps();
		TypeMaps() : this(DefaultParsingDelimiters.Default, AssemblyLoader.Default, TypePartitions.Default) {}

		readonly IParsingDelimiters _delimiters;
		readonly IAssemblyLoader _loader;
		readonly ITypePartitions _partitions;

		public TypeMaps(IParsingDelimiters delimiters, IAssemblyLoader loader, ITypePartitions partitions)
		{
			_delimiters = delimiters;
			_loader = loader;
			_partitions = partitions;
		}

		protected override ITypeMap Create(XNamespace parameter)
		{
			var parts = parameter.NamespaceName.ToStringArray(_delimiters.Part);
			var namespacePath = parts[0].ToStringArray(_delimiters.Namespace)[1];
			var delimiter = _delimiters.Assembly;
			var assemblyPath = string.Join(delimiter, parts[1].Split(delimiter).Skip(1));
			var assembly = _loader.Get(assemblyPath);
			var partition = _partitions.Get(assembly);
			var result = new Map(partition.Get(namespacePath), assembly, namespacePath);
			return result;
		}

		public TypeInfo Get(XName parameter) => Get(parameter.Namespace)?.Get(parameter.LocalName);

		sealed class Map : ITypeMap
		{
			readonly Assembly _assembly;
			readonly string _ns;
			readonly IAlteration<string> _names;
			readonly ITypeMap _map;

			public Map(ITypeMap map, Assembly assembly, string @namespace)
				: this(map, assembly, @namespace, TypeNameAlteration.Default) {}

			public Map(ITypeMap map, Assembly assembly, string @namespace, IAlteration<string> names)
			{
				_assembly = assembly;
				_ns = @namespace;
				_names = names;
				_map = map;
			}

			public TypeInfo Get(string parameter)
			{
				var result = _assembly.GetType($"{_ns}.{_names.Get(parameter)}", false, false)?.GetTypeInfo() ?? _map.Get(parameter);
				if (result == null)
				{
					throw new InvalidOperationException($"Could not find a type with the name '{parameter}' in assembly '{_assembly}'");
				}
				return result;
			}
		}
	}
}