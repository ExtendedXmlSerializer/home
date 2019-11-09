using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class InstanceAwareValueSpecification : DecoratedSpecification<object>, IInstanceValueSpecification
	{
		public InstanceAwareValueSpecification(ISpecification<object> specification, ISpecification<object> instance)
			: base(specification) => Instance = instance;

		public ISpecification<object> Instance { get; }
	}
}