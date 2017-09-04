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
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ConfigurationModel
{
	sealed class TypeConfigurations : CacheBase<TypeInfo, ITypeConfiguration>, ITypeConfigurations
	{
		readonly IRootContext _context;
		readonly IDictionary<TypeInfo, string> _names;

		public TypeConfigurations(IRootContext context) : this(context, context.Find<TypeNamesExtension>()
		                                                                       .Names) {}

		public TypeConfigurations(IRootContext context, IDictionary<TypeInfo, string> names)
		{
			_context = context;
			_names = names;
		}

		protected override ITypeConfiguration Create(TypeInfo parameter)
		{
			var property = new TypeProperty<string>(_names, parameter);
			var result = Source.Default
			                   .Get(parameter)
			                   .Invoke(_context, property);
			return result;
		}

		sealed class Source : Generic<IRootContext, IProperty<string>, ITypeConfiguration>
		{
			public static Source Default { get; } = new Source();
			Source() : base(typeof(TypeConfiguration<>)) {}
		}
	}
}