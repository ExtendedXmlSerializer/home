using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	class ListConstructorLocator : IParameterizedSource<TypeInfo, ConstructorInfo>
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly IConstructorLocator      _previous;
		readonly Type                     _baseType;

		public ListConstructorLocator(ISpecification<TypeInfo> specification, IConstructorLocator previous)
			: this(specification, previous, typeof(List<>)) {}

		public ListConstructorLocator(ISpecification<TypeInfo> specification, IConstructorLocator previous,
		                              Type baseType)
		{
			_specification = specification;
			_previous      = previous;
			_baseType      = baseType;
		}

		public ConstructorInfo Get(TypeInfo parameter)
		{
			var accounted = _specification.IsSatisfiedBy(parameter)
				                ? _baseType.MakeGenericType(CollectionItemTypeLocator.Default.Get(parameter))
				                : parameter;
			var result = _previous.Get(accounted);
			return result;
		}
	}
}