using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class AllowedAssignedInstanceValues : IAllowedMemberValues
	{
		readonly ISpecification<TypeInfo>                 _specification;
		readonly ITypeMemberDefaults                      _defaults;
		readonly IGeneric<object, ISpecification<object>> _generic;

		public AllowedAssignedInstanceValues(ITypeMemberDefaults defaults)
			: this(ActivatingTypeSpecification.Default, defaults,
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

			public Specification(IEnumerable<T> other) => _other = other ?? Enumerable.Empty<T>();

			public bool IsSatisfiedBy(object parameter)
				=> parameter is IEnumerable<T> other && other.SequenceEqual(_other);
		}
	}
}