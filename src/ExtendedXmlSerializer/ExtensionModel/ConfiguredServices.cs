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

using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class ConfiguredServices : IParameterizedSource<IEnumerable<ISerializerExtension>, IServices>
	{
		readonly static ServicesFactory ServicesFactory = ServicesFactory.Default;

		readonly ISource<IServices> _source;
		readonly IReadOnlyList<ISerializerExtension> _roots;

		public ConfiguredServices(params object[] instances)
			: this(
				ServicesFactory, new DefaultRegistrationsExtension(instances), ConverterExtension.Default,
				SerializationExtension.Default) {}

		public ConfiguredServices(ISource<IServices> source, params ISerializerExtension[] roots)
		{
			_source = source;
			_roots = roots.AsReadOnly();
		}

		public IServices Get(IEnumerable<ISerializerExtension> parameter)
		{
			var result = _source.Get();
			var input = parameter.ToArray();
			var extensions = _roots.Union(input, TypeEqualityComparer<ISerializerExtension>.Default).ToArray();
			extensions.Alter(result);

			foreach (var extension in extensions)
			{
				extension.Execute(result);
			}

			return result;
		}

		class TypeEqualityComparer<T> : IEqualityComparer<T>
		{
			public static TypeEqualityComparer<T> Default { get; } = new TypeEqualityComparer<T>();
			TypeEqualityComparer() {}

			public bool Equals(T x, T y) => x.GetType() == y.GetType();

			public int GetHashCode(T obj) => 0;
		}
	}
}