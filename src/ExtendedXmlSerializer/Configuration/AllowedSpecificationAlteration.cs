using System.Linq;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class AllowedSpecificationAlteration : IAlteration<AllowedMemberValuesExtension>
	{
		readonly IAllowedValueSpecification _specification;

		public AllowedSpecificationAlteration(IAllowedValueSpecification specification)
			=> _specification = specification;

		public AllowedMemberValuesExtension Get(AllowedMemberValuesExtension parameter)
			=> new AllowedMemberValuesExtension(_specification, parameter.Specifications, parameter.ToArray());
	}
}