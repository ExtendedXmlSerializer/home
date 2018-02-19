// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
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
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class GenericTypeConfigurations : IParameterizedSource<TypeInfo, ITypeConfiguration>
	{
		readonly IReflection _reflection;
		readonly IExtensions _extensions;
		readonly IGeneric<IReflection, IExtensions, ITypeConfiguration> _generic;

		public GenericTypeConfigurations(IReflection reflection, IExtensions extensions) : this(reflection, extensions, Generic.Default) {}

		public GenericTypeConfigurations(IReflection reflection, IExtensions extensions,
		                                 IGeneric<IReflection, IExtensions, ITypeConfiguration> generic)
		{
			_reflection = reflection;
			_extensions = extensions;
			_generic    = generic;
		}

		public ITypeConfiguration Get(TypeInfo parameter) => _generic.Get(parameter)
		                                                             .Invoke(_reflection, _extensions);

		sealed class Generic : Generic<IReflection, IExtensions, ITypeConfiguration>
		{
			public static Generic Default { get; } = new Generic();
			Generic() : base(typeof(TypeConfiguration<>)) {}
		}
	}
}