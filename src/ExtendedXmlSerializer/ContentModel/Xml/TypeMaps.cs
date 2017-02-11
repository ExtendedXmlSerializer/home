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
	class TypeMaps : ITypes
	{
		public static TypeMaps Default { get; } = new TypeMaps();

		TypeMaps()
			: this(DefaultParsingDelimiters.Default, AssemblyLoader.Default, TypePartitions.Default, TypeNameAlteration.Default) {}

		readonly IParsingDelimiters _delimiters;
		readonly IAssemblyLoader _loader;
		readonly ITypePartitions _partitions;
		readonly IAlteration<string> _names;

		public TypeMaps(IParsingDelimiters delimiters, IAssemblyLoader loader, ITypePartitions partitions,
		                IAlteration<string> names)
		{
			_delimiters = delimiters;
			_loader = loader;
			_partitions = partitions;
			_names = names;
		}

		public TypeInfo Get(XName parameter)
		{
			var parts = parameter.NamespaceName.ToStringArray(_delimiters.Part);
			var namespacePath = parts[0].ToStringArray(_delimiters.Namespace)[1];
			var delimiter = _delimiters.Assembly;
			var assemblyPath = string.Join(delimiter, parts[1].Split(delimiter).Skip(1));
			var assembly = _loader.Get(assemblyPath);

			var result = assembly.GetType($"{namespacePath}.{_names.Get(parameter.LocalName)}", false, false)?.GetTypeInfo() ??
			             _partitions.Get(assembly).Invoke(namespacePath).Invoke(parameter.LocalName);
			if (result == null)
			{
				throw new InvalidOperationException($"Could not find a type with the name '{parameter}' in assembly '{assembly}'");
			}
			return result;
		}
	}
}