using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class VariableTypeSpecification : DecoratedSpecification<Type>, IVariableTypeSpecification
	{
		public static IParameterizedSource<Type, IVariableTypeSpecification> Defaults { get; }
			= new ReferenceCache<Type, IVariableTypeSpecification>(x => new VariableTypeSpecification(x));

		public VariableTypeSpecification(Type type)
			: base(new DelegatedSpecification<Type>(IsAssignableSpecification.Defaults.Get(type)
			                                                                 .IsSatisfiedBy)
				       .And(new EqualitySpecification<Type>(type).Inverse())) {}

		bool ISpecification<TypeInfo>.IsSatisfiedBy(TypeInfo parameter) => IsSatisfiedBy(parameter.AsType());
	}
}