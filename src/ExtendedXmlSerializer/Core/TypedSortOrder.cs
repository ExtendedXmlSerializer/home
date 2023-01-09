using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.Core
{
	sealed class TypedSortOrder : StructureCache<TypeInfo, int>, ITypedSortOrder
	{
		public TypedSortOrder() : base(_ => 1) {}
	}
}