using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	interface IRuntimeSerializer : IParameterizedSource<object, IMemberSerializer>, IMemberSerializer {}
}