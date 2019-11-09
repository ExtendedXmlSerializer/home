using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	interface ITypeCandidates : IParameterizedSource<IIdentity, ImmutableArray<TypeInfo>> {}
}