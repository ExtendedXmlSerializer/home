using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class TypeDefinitionIdentityComparer : TypeIdentityComparerBase
	{
		public static TypeDefinitionIdentityComparer Default { get; } = new TypeDefinitionIdentityComparer();

		TypeDefinitionIdentityComparer() {}

		public override int GetHashCode(TypeInfo obj) => obj.GUID.GetHashCode();
	}
}