using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class AddAlteration : IAlteration<AllowedMemberValuesExtension>
	{
		readonly IAllowedMemberValues _add;

		public AddAlteration(IAllowedMemberValues add) => _add = add;

		public AllowedMemberValuesExtension Get(AllowedMemberValuesExtension parameter)
		{
			parameter.Add(_add);
			return parameter;
		}
	}
}