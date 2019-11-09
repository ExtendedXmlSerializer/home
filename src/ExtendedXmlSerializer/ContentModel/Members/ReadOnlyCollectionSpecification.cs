using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class ReadOnlyCollectionSpecification : DecoratedSpecification<IMember>
	{
		public static ReadOnlyCollectionSpecification Default { get; } = new ReadOnlyCollectionSpecification();

		ReadOnlyCollectionSpecification() : this(AddDelegates.Default) {}

		public ReadOnlyCollectionSpecification(IAddDelegates add)
			: base(new MemberTypeSpecification(IsCollectionTypeSpecification.Default.And(add.IfAssigned()))) {}
	}
}