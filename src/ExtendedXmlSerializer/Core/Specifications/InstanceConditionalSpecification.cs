using JetBrains.Annotations;

namespace ExtendedXmlSerializer.Core.Specifications
{
	sealed class InstanceConditionalSpecification : ISpecification<object>
	{
		readonly IConditions _conditions;

		[UsedImplicitly]
		public InstanceConditionalSpecification() : this(new Conditions()) {}

		public InstanceConditionalSpecification(IConditions conditions) => _conditions = conditions;

		public bool IsSatisfiedBy(object parameter) => _conditions.Get(parameter)
		                                                          .Apply();
	}
}