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
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ConverterModel.Xml
{
	class WellKnownTypeLocator : ITypes
	{
		public static WellKnownTypeLocator Default { get; } = new WellKnownTypeLocator();

		WellKnownTypeLocator()
			: this(
				WellKnownNamespaces.Default.ToDictionary(x => x.Value.Identifier,
				                                         x => new Locator(SearchableTypes.Default.Get(x.Key)).ToDelegate())
			) {}

		readonly IDictionary<string, Func<string, TypeInfo>> _types;

		public WellKnownTypeLocator(IDictionary<string, Func<string, TypeInfo>> types)
		{
			_types = types;
		}

		public TypeInfo Get(XName parameter) => _types.Get(parameter.NamespaceName)?.Invoke(parameter.LocalName);

		sealed class Locator : IParameterizedSource<string, TypeInfo>
		{
			readonly IReadOnlyList<TypeInfo> _types;
			readonly IDictionary<string, TypeInfo> _lookup;

			public Locator(IReadOnlyList<TypeInfo> types)
				: this(types, types.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.First())) {}

			public Locator(IReadOnlyList<TypeInfo> types, IDictionary<string, TypeInfo> lookup)
			{
				_types = types;
				_lookup = lookup;
			}

			public TypeInfo Get(string parameter) => _lookup.Get(parameter) ?? Search(_types, parameter);

			static TypeInfo Search(IReadOnlyList<TypeInfo> types, string parameter)
			{
				var length = types.Count;
				for (var i = 0; i < length; i++)
				{
					var type = types[i];
					if (type.Name.StartsWith(parameter))
					{
						return type;
					}
				}

				throw new InvalidOperationException(
					$"Could not find a type with the name '{parameter}' in array '{types[0].Assembly}'");
			}
		}
	}
}