using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	class DictionaryConstructorLocator : IParameterizedSource<TypeInfo, ConstructorInfo>
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly IConstructorLocator      _previous;
		readonly Type                     _baseType;

		public DictionaryConstructorLocator(ISpecification<TypeInfo> specification, IConstructorLocator previous)
			: this(specification, previous, typeof(Dictionary<,>)) {}

		public DictionaryConstructorLocator(ISpecification<TypeInfo> specification, IConstructorLocator previous,
		                                    Type baseType)
		{
			_specification = specification;
			_previous      = previous;
			_baseType      = baseType;
		}

		public ConstructorInfo Get(TypeInfo parameter)
		{
			var accounted = _specification.IsSatisfiedBy(parameter)
				                ? MakeGenericType(parameter)
				                : parameter;
			var result = _previous.Get(accounted);
			return result;
		}

		Type MakeGenericType(TypeInfo parameter)
		{
			var types  = DictionaryPairTypesLocator.Default.Get(parameter);
			var result = _baseType.MakeGenericType(types?.KeyType, types?.ValueType);
			return result;
		}
	}
}