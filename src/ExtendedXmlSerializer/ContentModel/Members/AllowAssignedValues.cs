using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class AllowAssignedValues : AllowedValueSpecification
	{
		public static AllowAssignedValues Default { get; } = new AllowAssignedValues();

		AllowAssignedValues() : base(AssignedSpecification<object>.Default) {}
	}
}