using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class IsGenericTypeSpecification : ISpecification<TypeInfo>
	{
		public static IsGenericTypeSpecification Default { get; } = new IsGenericTypeSpecification();

		IsGenericTypeSpecification() {}

		public bool IsSatisfiedBy(TypeInfo parameter) => parameter.IsGenericType;
	}
}