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
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ConverterModel.Xml
{
	class TypePartitions : WeakCacheBase<string, Func<string, TypeInfo>>, ITypePartitions
	{
		public static TypePartitions Default { get; } = new TypePartitions();
		TypePartitions() : this(DefaultParsingDelimiters.Default, AssemblyLoader.Default, TypeNameAlteration.Default) {}

		readonly IParsingDelimiters _delimiters;
		readonly IAssemblyLoader _loader;
		readonly IAlteration<string> _alteration;

		public TypePartitions(IParsingDelimiters delimiters, IAssemblyLoader loader, IAlteration<string> alteration)
		{
			_delimiters = delimiters;
			_loader = loader;
			_alteration = alteration;
		}

		protected override Func<string, TypeInfo> Create(string parameter)
		{
			var parts = parameter.ToStringArray(_delimiters.Part);
			var namespacePath = parts[0].ToStringArray(_delimiters.Namespace)[1];
			var assemblyPath = string.Join(_delimiters.Assembly, parts[1].Split(_delimiters.Assembly).Skip(1));
			var assembly = _loader.Get(assemblyPath);
			var result = new Locator(assembly, namespacePath, _alteration).ToDelegate();
			return result;
		}

		sealed class Locator : IParameterizedSource<string, TypeInfo>
		{
			readonly Assembly _assembly;
			readonly string _ns;
			readonly IAlteration<string> _alteration;
			readonly Func<ImmutableArray<TypeInfo>> _types;

			public Locator(Assembly assembly, string @namespace, IAlteration<string> alteration)
				: this(assembly, @namespace, alteration, new Namespaces(SearchableTypes.Default.Build(assembly)).Build(@namespace)) {}

			public Locator(Assembly assembly, string @namespace, IAlteration<string> alteration,
			               Func<ImmutableArray<TypeInfo>> types)
			{
				_assembly = assembly;
				_ns = @namespace;
				_alteration = alteration;
				_types = types;
			}

			public TypeInfo Get(string parameter) => Locate($"{_ns}.{_alteration.Get(parameter)}");

			TypeInfo Locate(string parameter) => _assembly.GetType(parameter, false, false)?.GetTypeInfo() ?? Search(parameter);

			TypeInfo Search(string parameter)
			{
				var types = _types();
				for (var i = 0; i < types.Length; i++)
				{
					var type = types[i];
					if (type.FullName.StartsWith(parameter))
					{
						return type;
					}
				}
				return null;
			}
		}

		sealed class Namespaces : IParameterizedSource<string, ImmutableArray<TypeInfo>>
		{
			readonly Func<ImmutableArray<TypeInfo>> _types;

			public Namespaces(Func<ImmutableArray<TypeInfo>> types)
			{
				_types = types;
			}

			public ImmutableArray<TypeInfo> Get(string parameter) => Yield(parameter).ToImmutableArray();

			IEnumerable<TypeInfo> Yield(string parameter)
			{
				var types = _types();
				var length = types.Length;
				for (var i = 0; i < length; i++)
				{
					var type = types[i];
					if (type.Namespace == parameter)
					{
						yield return type;
					}
				}
			}
		}
	}
}