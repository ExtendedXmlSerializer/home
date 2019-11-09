using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	interface IMetadataLocator : IParameterizedSource<string, MemberDescriptor?> {}
}