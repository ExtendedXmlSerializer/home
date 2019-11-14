using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class DiscoveredTypes : StructureCacheBase<TypeInfo, ImmutableArray<TypeInfo>>, IDiscoveredTypes
	{
		readonly ITypeMembers _members;

		public DiscoveredTypes(ITypeMembers members) => _members = members;

		protected override ImmutableArray<TypeInfo> Create(TypeInfo parameter)
			=> new VariableTypeWalker(_members, parameter).Get().ToImmutableArray();
	}
}