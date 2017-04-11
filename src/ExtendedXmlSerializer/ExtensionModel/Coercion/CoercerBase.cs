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

using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Coercion
{
	abstract class CoercerBase<TFrom, TTo> : ICoercer
	{
		readonly static ISpecification<object> From = IsInstanceOfTypeSpecification<TFrom>.Default;
		readonly static ISpecification<TypeInfo> To = IsAssignableSpecification<TTo>.Default;

		readonly ISpecification<object> _from;
		readonly ISpecification<TypeInfo> _to;

		protected CoercerBase() : this(To) {}

		protected CoercerBase(ISpecification<TypeInfo> to) : this(From, to) {}

		protected CoercerBase(ISpecification<object> from, ISpecification<TypeInfo> to)
		{
			_from = from;
			_to = to;
		}

		public bool IsSatisfiedBy(object parameter) => _from.IsSatisfiedBy(parameter);
		public bool IsSatisfiedBy(TypeInfo parameter) => _to.IsSatisfiedBy(parameter);

		protected abstract TTo Get(TFrom parameter, TypeInfo targetType);

		object IParameterizedSource<CoercerParameter, object>.Get(CoercerParameter parameter)
			=> Get((TFrom) parameter.Instance, parameter.TargetType);
	}
}