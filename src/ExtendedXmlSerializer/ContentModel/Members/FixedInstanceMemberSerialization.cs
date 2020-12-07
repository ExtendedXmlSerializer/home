using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class FixedInstanceMemberSerialization
		: FixedInstanceSource<object, IMemberSerialization>, IInstanceMemberSerialization
	{
		public FixedInstanceMemberSerialization(IMemberSerialization instance) : base(instance) {}

		public IMemberSerialization Get(IInnerContent parameter) => Get(parameter.Current);
	}
}