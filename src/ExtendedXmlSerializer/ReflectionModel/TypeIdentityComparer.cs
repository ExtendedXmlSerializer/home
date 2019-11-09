using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class TypeIdentityComparer : TypeIdentityComparerBase
	{
		public static TypeIdentityComparer Default { get; } = new TypeIdentityComparer();

		TypeIdentityComparer() {}

		public override int GetHashCode(TypeInfo obj) => obj.GetHashCode();
	}
}