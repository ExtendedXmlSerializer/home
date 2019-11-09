using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class MemberSpecifications : Items<ISpecification<IMember>>, IMemberSpecifications
	{
		public MemberSpecifications(IsValidMemberType valid) : base(IsField.Default.Or(IsWritable.Default.Inverse()),
		                                                            valid) {}
	}
}