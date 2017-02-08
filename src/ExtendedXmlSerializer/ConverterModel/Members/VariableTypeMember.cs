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
using System.Reflection;
using ExtendedXmlSerialization.ConverterModel.Elements;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ConverterModel.Members
{
	class VariableTypeMember : Member, IVariableTypeMember
	{
		readonly ISpecification<Type> _specification;

		public VariableTypeMember(string displayName, TypeInfo classification, Func<object, object> getter,
		                          Action<object, object> setter, IConverter runtime, IConverter body)
			: this(displayName, getter, setter, new EqualitySpecification<Type>(classification.AsType()).Inverse(), runtime, body
			) {}

		VariableTypeMember(string displayName, Func<object, object> getter, Action<object, object> setter,
		                   ISpecification<Type> specification, IConverter runtime, IConverter body)
			: base(
				displayName, getter, setter, new DecoratedConverter(body, new VariableTypeWriter(specification, runtime, body)))
		{
			_specification = specification;
		}

		public bool IsSatisfiedBy(Type parameter) => _specification.IsSatisfiedBy(parameter);
	}
}