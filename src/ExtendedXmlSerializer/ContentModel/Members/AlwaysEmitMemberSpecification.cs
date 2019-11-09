using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	class AlwaysEmitMemberSpecification : AllowedValueSpecification
	{
		public static AlwaysEmitMemberSpecification Default { get; } = new AlwaysEmitMemberSpecification();

		AlwaysEmitMemberSpecification() : base(AlwaysSpecification<object>.Default) {}
	}
}