using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class AllMemberSpecifications : Items<ISpecification<IMember>>, IMemberSpecifications
	{
		public AllMemberSpecifications(IsValidMemberType specification) : base(specification) {}
	}
}