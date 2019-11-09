using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class IsAssignableGenericSpecification : ISpecification<TypeInfo>
	{
		readonly static GenericDefinitionCandidates Candidates = GenericDefinitionCandidates.Default;

		readonly IGenericDefinitionCandidates _candidates;
		readonly Type                         _genericDefinition;

		public IsAssignableGenericSpecification(Type genericType) : this(Candidates, genericType) {}

		public IsAssignableGenericSpecification(IGenericDefinitionCandidates candidates, Type genericDefinition)
		{
			_candidates        = candidates;
			_genericDefinition = genericDefinition;
		}

		public bool IsSatisfiedBy(TypeInfo parameter)
			=> _candidates.Get(parameter)
			              .Contains(_genericDefinition) || Base(parameter);

		bool Base(TypeInfo parameter)
		{
			var baseType = parameter.BaseType?.GetTypeInfo();
			var result   = baseType != null && IsSatisfiedBy(baseType);
			return result;
		}
	}
}