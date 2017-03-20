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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.TypeModel
{
	sealed class ApplicationTypes : CacheBase<Assembly, ImmutableArray<TypeInfo>>, IApplicationTypes
	{
		readonly static Func<TypeInfo, bool> Specification = ApplicationTypeSpecification.Default.IsSatisfiedBy;

		public static ApplicationTypes Default { get; } = new ApplicationTypes();
		ApplicationTypes() : this(AssemblyPublicTypes.Default) {}

		public static ApplicationTypes All { get; } = new ApplicationTypes(AssemblyTypes.Default);

		readonly Func<TypeInfo, bool> _specification;
		readonly IAssemblyTypes _types;

		public ApplicationTypes(IAssemblyTypes types) : this(Specification, types) {}

		public ApplicationTypes(Func<TypeInfo, bool> specification, IAssemblyTypes types)
		{
			_specification = specification;
			_types = types;
		}

		protected override ImmutableArray<TypeInfo> Create(Assembly parameter) => _types.Get(parameter)
		                                                                               .Where(_specification)
		                                                                               .ToImmutableArray();
	}
}