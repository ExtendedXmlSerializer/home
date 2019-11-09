using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class FixedInstanceMemberSerialization
		: FixedInstanceSource<object, IMemberSerialization>, IInstanceMemberSerialization
	{
		public FixedInstanceMemberSerialization(IMemberSerialization instance) : base(instance) {}
	}
}