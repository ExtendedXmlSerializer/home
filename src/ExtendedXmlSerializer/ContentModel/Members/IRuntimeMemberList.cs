using System.Collections.Immutable;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	interface IRuntimeMemberList : IParameterizedSource<object, ImmutableArray<IMemberSerializer>> {}
}