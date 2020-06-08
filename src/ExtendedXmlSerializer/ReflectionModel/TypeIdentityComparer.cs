using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <summary>
	/// Type comparer based on hash code.
	/// </summary>
	public sealed class TypeIdentityComparer : TypeIdentityComparerBase
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static TypeIdentityComparer Default { get; } = new TypeIdentityComparer();

		TypeIdentityComparer() {}

		/// <inheritdoc />
		public override int GetHashCode(TypeInfo obj) => obj.GetHashCode();
	}
}