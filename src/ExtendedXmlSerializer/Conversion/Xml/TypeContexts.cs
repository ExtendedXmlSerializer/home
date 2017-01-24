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
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class TypeContexts : CacheBase<string, Func<string, TypeInfo>>, ITypeContexts
	{
		readonly static char[] Delimiters = Defaults.PartDelimiter.ToArray(),
			Separator = Defaults.NamespaceDelimiter.ToArray(),
			AssemblySeparator = Defaults.AssemblyDelimiter.ToArray();

		public TypeContexts() : this(AssemblyLoader.Default, TypeNameAlteration.Default) {}

		readonly IAssemblyLoader _loader;
		readonly IAlteration<string> _alteration;

		public TypeContexts(IAssemblyLoader loader, IAlteration<string> alteration)
		{
			_loader = loader;
			_alteration = alteration;
		}

		protected override Func<string, TypeInfo> Create(string parameter)
		{
			var parts = parameter.ToStringArray(Delimiters);
			var namespacePath = parts[0].Split(Separator)[1];
			var assemblyPath = parts[1].Split(AssemblySeparator)[1];
			var assembly = _loader.Get(assemblyPath);
			var result = new TypeLoaderContext(assembly, namespacePath, _alteration).ToDelegate();
			return result;
		}

		sealed class TypeLoaderContext : CacheBase<string, TypeInfo>
		{
			readonly Assembly _assembly;
			readonly string _ns;
			readonly IAlteration<string> _alteration;

			public TypeLoaderContext(Assembly assembly, string @namespace, IAlteration<string> alteration)
			{
				_assembly = assembly;
				_ns = @namespace;
				_alteration = alteration;
			}

			protected override TypeInfo Create(string parameter)
				=> _assembly.GetType($"{_ns}.{_alteration.Get(parameter)}", false, false)?.GetTypeInfo();
		}
	}
}