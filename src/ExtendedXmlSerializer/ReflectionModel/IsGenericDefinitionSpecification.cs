using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class IsGenericDefinitionSpecification : ISpecification<TypeInfo>
	{
		readonly Type _genericType;

		public IsGenericDefinitionSpecification(Type genericType) => _genericType = genericType;

		public bool IsSatisfiedBy(TypeInfo parameter) => parameter.GetGenericTypeDefinition() == _genericType;
	}
}