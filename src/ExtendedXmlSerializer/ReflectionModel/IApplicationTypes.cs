using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IApplicationTypes : IParameterizedSource<Assembly, ImmutableArray<TypeInfo>> {}
}