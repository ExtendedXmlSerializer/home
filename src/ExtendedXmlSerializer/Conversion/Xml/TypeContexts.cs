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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class TypeContexts : CacheBase<string, Func<string, TypeInfo>>, ITypeContexts
	{
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
			var parts = parameter.ToStringArray(DefaultParsingDelimiters.Default.Part);
			var namespacePath = parts[0].Split(DefaultParsingDelimiters.Default.Namespace)[1];
			var assemblyPath = parts[1].Split(DefaultParsingDelimiters.Default.Assembly)[1];
			var assembly = _loader.Get(assemblyPath);
			var result = new TypeLoaderContext(assembly, namespacePath, _alteration).ToDelegate();
			return result;
		}

		sealed class TypeLoaderContext : CacheBase<string, TypeInfo>
		{
			readonly Assembly _assembly;
			readonly string _ns;
			readonly IAlteration<string> _alteration;
			readonly Func<ImmutableArray<TypeInfo>> _types;

			public TypeLoaderContext(Assembly assembly, string @namespace, IAlteration<string> alteration)
				: this(assembly, @namespace, alteration, new Types(assembly).Build(@namespace)) {}

			public TypeLoaderContext(Assembly assembly, string @namespace, IAlteration<string> alteration,
			                         Func<ImmutableArray<TypeInfo>> types)
			{
				_assembly = assembly;
				_ns = @namespace;
				_alteration = alteration;
				_types = types;
			}

			protected override TypeInfo Create(string parameter) => Locate($"{_ns}.{_alteration.Get(parameter)}");

			TypeInfo Locate(string parameter) => _assembly.GetType(parameter, false, false)?.GetTypeInfo() ?? Search(parameter);

			TypeInfo Search(string parameter)
			{
				foreach (var typeInfo in _types())
				{
					if (typeInfo.FullName.StartsWith(parameter))
					{
						return typeInfo;
					}
				}
				return null;
			}
		}

		sealed class Types : CacheBase<string, ImmutableArray<TypeInfo>>
		{
			readonly ImmutableArray<TypeInfo> _types;

			public Types(Assembly assembly) : this(assembly.DefinedTypes.ToImmutableArray()) {}

			public Types(ImmutableArray<TypeInfo> types)
			{
				_types = types;
			}

			protected override ImmutableArray<TypeInfo> Create(string parameter) => Yield(parameter).ToImmutableArray();

			IEnumerable<TypeInfo> Yield(string parameter)
			{
				foreach (var typeInfo in _types)
				{
					if (typeInfo.Namespace == parameter)
					{
						yield return typeInfo;
					}
				}
			}
		}
	}
}