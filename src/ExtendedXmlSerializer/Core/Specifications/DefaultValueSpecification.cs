namespace ExtendedXmlSerializer.Core.Specifications
{
	sealed class DefaultValueSpecification<T> : ISpecification<T>
	{
		public static DefaultValueSpecification<T> Default { get; } = new DefaultValueSpecification<T>();

		DefaultValueSpecification() : this(default) {}

		readonly T _default;

		public DefaultValueSpecification(T @default) => _default = @default;

		public bool IsSatisfiedBy(T parameter) => Equals(_default, parameter);
	}
}