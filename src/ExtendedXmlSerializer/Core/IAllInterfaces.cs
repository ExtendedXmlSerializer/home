using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Core
{
	interface IAllInterfaces : IParameterizedSource<TypeInfo, ImmutableArray<TypeInfo>> {}
}