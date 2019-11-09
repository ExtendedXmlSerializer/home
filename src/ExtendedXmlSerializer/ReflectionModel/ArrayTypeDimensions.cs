using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ArrayTypeDimensions : StructureCacheBase<TypeInfo, ImmutableArray<int>>
	{
		public static ArrayTypeDimensions Default { get; } = new ArrayTypeDimensions();

		ArrayTypeDimensions() {}

		protected override ImmutableArray<int> Create(TypeInfo parameter)
			=> new ArrayDimensions(parameter).ToImmutableArray();
	}
}