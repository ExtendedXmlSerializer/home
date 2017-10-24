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

using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class TypeConfigurations : CacheBase<TypeInfo, ITypeConfiguration>, ITypeConfigurations
	{
		readonly IExtensionCollection _extensions;
		readonly IDictionary<TypeInfo, string> _names;
		readonly ConcurrentDictionary<TypeInfo, ITypeConfiguration> _store;

		public TypeConfigurations(IExtensionCollection extensions)
			: this(extensions, extensions.Find<TypeNamesExtension>()
			                             .Names, new ConcurrentDictionary<TypeInfo, ITypeConfiguration>()) {}

		public TypeConfigurations(IExtensionCollection extensions, IDictionary<TypeInfo, string> names,
		                          ConcurrentDictionary<TypeInfo, ITypeConfiguration> store) : base(store)
		{
			_extensions = extensions;
			_names = names;
			_store = store;
		}

		protected override ITypeConfiguration Create(TypeInfo parameter)
		{
			var property = new TypeProperty<string>(_names, parameter);
			var root = _extensions.Find<RootContextExtension>()
			                      .Root;
			var result = Source.Default
			                   .Get(parameter)
			                   .Invoke(root, property);
			return result;
		}

		sealed class Source : Generic<IRootContext, IProperty<string>, ITypeConfiguration>
		{
			public static Source Default { get; } = new Source();
			Source() : base(typeof(TypeConfiguration<>)) {}
		}

		public IEnumerator<ITypeConfiguration> GetEnumerator() => _store.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}