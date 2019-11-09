using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	class IsValueTypeSpecification : ISpecification<TypeInfo>
	{
		public static IsValueTypeSpecification Default { get; } = new IsValueTypeSpecification();

		IsValueTypeSpecification() {}

		public bool IsSatisfiedBy(TypeInfo parameter) => parameter.IsValueType;
	}
}