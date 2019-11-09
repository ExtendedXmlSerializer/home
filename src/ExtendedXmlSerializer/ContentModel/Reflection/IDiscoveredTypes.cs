using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	interface IDiscoveredTypes : IParameterizedSource<TypeInfo, ImmutableArray<TypeInfo>> {}
}