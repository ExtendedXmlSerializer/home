using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	class ListConstructorLocator : IParameterizedSource<TypeInfo, ConstructorInfo>
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly IConstructorLocator      _previous;

		public ListConstructorLocator(ISpecification<TypeInfo> specification, IConstructorLocator previous)
		{
			_specification = specification;
			_previous      = previous;
		}

		public ConstructorInfo Get(TypeInfo parameter)
		{
			var accounted = _specification.IsSatisfiedBy(parameter)
				                ? typeof(List<>).MakeGenericType(CollectionItemTypeLocator.Default.Get(parameter))
				                : parameter;
			var result = _previous.Get(accounted);
			return result;
		}
	}
}