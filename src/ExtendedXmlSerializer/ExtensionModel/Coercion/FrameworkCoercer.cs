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
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Coercion
{
	sealed class FrameworkCoercer : CoercerBase<IConvertible, object>
	{
		readonly static ISpecification<TypeInfo> Supported = new ContainsSpecification<TypeInfo>(
			new HashSet<TypeInfo>(new[]
			                      {
				                      typeof(bool), typeof(char), typeof(sbyte), typeof(byte), typeof(short),
				                      typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float),
				                      typeof(double), typeof(decimal), typeof(DateTimeOffset), typeof(DateTime), typeof(string)
			                      }.YieldMetadata()));

		public static FrameworkCoercer Default { get; } = new FrameworkCoercer();
		FrameworkCoercer() : this(Supported) {}

		public FrameworkCoercer(ISpecification<TypeInfo> to) : base(to) {}

		protected override object Get(IConvertible parameter, TypeInfo targetType)
			=> Convert.ChangeType(parameter, targetType.AsType());
	}
}