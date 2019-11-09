using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Core
{
	sealed class TypedSortOrder : StructureCache<TypeInfo, int>, ITypedSortOrder
	{
		public TypedSortOrder() : base(info => 1) {}
	}
}