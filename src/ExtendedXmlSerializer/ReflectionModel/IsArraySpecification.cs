using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class IsArraySpecification : ISpecification<TypeInfo>
	{
		public static IsArraySpecification Default { get; } = new IsArraySpecification();

		IsArraySpecification() {}

		public bool IsSatisfiedBy(TypeInfo parameter) => parameter.IsArray;
	}
}