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
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ExtensionModel;

namespace ExtendedXmlSerialization.Configuration
{
	sealed class TypeConfigurations : ReferenceCacheBase<TypeInfo, ITypeConfiguration>
	{
		public static IParameterizedSource<IConfiguration, TypeConfigurations> Defaults { get; }
			= new ReferenceCache<IConfiguration, TypeConfigurations>(x => new TypeConfigurations(x));

		readonly IConfiguration _configuration;
		readonly IDictionary<TypeInfo, string> _names;

		TypeConfigurations(IConfiguration configuration)
			: this(configuration, configuration.With<TypeNamesExtension>().Names) {}

		TypeConfigurations(IConfiguration configuration, IDictionary<TypeInfo, string> names)
		{
			_configuration = configuration;
			_names = names;
		}

		protected override ITypeConfiguration Create(TypeInfo parameter)
			=> new TypeConfiguration(_configuration, new TypeProperty<string>(_names, parameter), parameter);
	}

	sealed class TypeConfigurations<T> : ReferenceCache<IConfiguration, TypeConfiguration<T>>
	{
		public static TypeConfigurations<T> Default { get; } = new TypeConfigurations<T>();
		TypeConfigurations() : base(x => new TypeConfiguration<T>(x)) {}

		public TypeConfiguration<T> For(ITypeConfiguration type)
			=> type as TypeConfiguration<T> ?? Get(type.Configuration);
	}
}