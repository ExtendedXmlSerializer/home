namespace ExtendedXmlSerializer.Core.Specifications
{
	sealed class ConditionalSpecification<T> : ISpecification<T>
	{
		readonly ConditionMonitor _monitor;

		public ConditionalSpecification() : this(new ConditionMonitor()) {}

		public ConditionalSpecification(ConditionMonitor monitor) => _monitor = monitor;

		public bool IsSatisfiedBy(T parameter) => _monitor.Apply();
	}
}