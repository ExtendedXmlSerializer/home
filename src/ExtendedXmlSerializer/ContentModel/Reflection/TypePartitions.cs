using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypePartitions : FirstAssignedSource<TypePartition, ImmutableArray<TypeInfo>?>, ITypePartitions
	{
		public TypePartitions(params IParameterizedSource<TypePartition, ImmutableArray<TypeInfo>?>[] sources) :
			base(sources) {}
	}
}