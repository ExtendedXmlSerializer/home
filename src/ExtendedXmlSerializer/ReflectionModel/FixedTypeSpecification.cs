using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class FixedTypeSpecification : ISpecification<TypeInfo>
	{
		public static FixedTypeSpecification Default { get; } = new FixedTypeSpecification();

		FixedTypeSpecification() {}

		public bool IsSatisfiedBy(TypeInfo parameter)
			=> parameter.IsPrimitive || parameter.IsValueType || parameter.IsSealed;
	}
}