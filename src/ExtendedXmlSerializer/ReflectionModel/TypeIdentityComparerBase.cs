using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <summary>
	/// A baseline implementation of <see cref="ITypeComparer"/> for type comparison.
	/// </summary>
	[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute", Justification = "Using framework code.")]
	public abstract class TypeIdentityComparerBase : ITypeComparer
	{
		/// <inheritdoc />
		public virtual bool Equals(TypeInfo x, TypeInfo y) => GetHashCode(x).Equals(GetHashCode(y));

		/// <inheritdoc />
		public abstract int GetHashCode(TypeInfo obj);
	}
}