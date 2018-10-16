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

using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	class AllowedAssignedInstanceValues : IAllowedMemberValues
	{
		readonly ISpecification<TypeInfo>                 _specification;
		readonly ITypeMemberDefaults                      _defaults;
		readonly IGeneric<object, ISpecification<object>> _generic;

		public static AllowedAssignedInstanceValues Default { get; } = new AllowedAssignedInstanceValues();

		AllowedAssignedInstanceValues()
			: this(ActivatingTypeSpecification.Default, TypeMemberDefaults.Default,
			       new Generic<object, ISpecification<object>>(typeof(Specification<>))) {}

		public AllowedAssignedInstanceValues(ISpecification<TypeInfo> specification, ITypeMemberDefaults defaults,
		                                     IGeneric<object, ISpecification<object>> generic)
		{
			_specification = specification;
			_defaults      = defaults;
			_generic       = generic;
		}

		public IAllowedValueSpecification Get(MemberInfo parameter)
		{
			var type   = parameter.ReflectedType.GetTypeInfo();
			var result = _specification.IsSatisfiedBy(type) ? FromDefault(type, parameter) : null;
			return result;
		}

		IAllowedValueSpecification FromDefault(TypeInfo reflectedType, MemberDescriptor parameter)
		{
			var defaultValue = _defaults.Get(reflectedType)
			                            .Invoke(parameter.Metadata);

			var specification = IsCollectionTypeSpecification.Default.IsSatisfiedBy(parameter.MemberType)
				                    ? _generic.Get(CollectionItemTypeLocator.Default.Get(parameter.MemberType))
				                              .Invoke(defaultValue)
				                    : new EqualitySpecification<object>(defaultValue);

			var result = new AllowedValueSpecification(specification.Inverse());
			return result;
		}

		sealed class Specification<T> : ISpecification<object>
		{
			readonly IEnumerable<T> _other;

			public Specification(IEnumerable<T> other) => _other = other;

			public bool IsSatisfiedBy(object parameter)
				=> parameter is IEnumerable<T> other && other.SequenceEqual(_other);
		}
	}
}