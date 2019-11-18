using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <summary>
	/// Specification to determine if a given object is of the referenced type.
	/// </summary>
	/// <typeparam name="T">The referenced type.</typeparam>
	public sealed class IsTypeSpecification<T> : ISpecification<object>
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static IsTypeSpecification<T> Default { get; } = new IsTypeSpecification<T>();

		IsTypeSpecification() {}

		/// <inheritdoc />
		public bool IsSatisfiedBy(object parameter) => parameter is T;
	}
}