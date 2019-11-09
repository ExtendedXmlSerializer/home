using System.Linq;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class MemberSpecification : AllSpecification<IMember>, IMemberSpecification
	{
		public MemberSpecification(IMemberSpecifications specifications) : base(specifications.ToArray()) {}
	}
}