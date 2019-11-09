using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	public sealed class IsTypeSpecification<T> : ISpecification<object>
	{
		public static IsTypeSpecification<T> Default { get; } = new IsTypeSpecification<T>();

		IsTypeSpecification() {}

		public bool IsSatisfiedBy(object parameter) => parameter is T;
	}
}