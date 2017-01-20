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
using System.Collections;
using System.Reflection;
using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ElementModel
{
	sealed class EnumerableNameProvider : NameProvider
	{
		public new static EnumerableNameProvider Default { get; } = new EnumerableNameProvider();

		EnumerableNameProvider() : base(EnumerableTypeFormatter.Default) {}
	}

	public class EnumerableNameOption : ElementNameOptionBase
	{
		public static EnumerableNameOption Default { get; } = new EnumerableNameOption();
		EnumerableNameOption() : this(EnumerableNameProvider.Default.Get) {}

		public EnumerableNameOption(Func<MemberInfo, string> source) : base(Specification.Instance, source) {}

		sealed class Specification : ISpecification<MemberInfo>
		{
			readonly static TypeInfo TypeInfo = typeof(IEnumerable).GetTypeInfo();

			public static Specification Instance { get; } = new Specification();
			Specification() {}

			public bool IsSatisfiedBy(MemberInfo parameter)
			{
				var type = parameter.ToTypeInfo();
				var result = type.IsArray || type.IsGenericType && TypeInfo.IsAssignableFrom(type);
				return result;
			}
		}
	}
}