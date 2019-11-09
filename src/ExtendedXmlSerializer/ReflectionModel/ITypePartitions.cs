using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface ITypePartitions : IParameterizedSource<TypePartition, ImmutableArray<TypeInfo>?> {}
}