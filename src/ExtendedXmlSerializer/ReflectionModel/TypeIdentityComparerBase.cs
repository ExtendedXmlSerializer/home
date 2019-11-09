using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute", Justification = "Using framework code.")]
	public abstract class TypeIdentityComparerBase : ITypeComparer
	{
		public virtual bool Equals(TypeInfo x, TypeInfo y) => GetHashCode(x).Equals(GetHashCode(y));

		public abstract int GetHashCode(TypeInfo obj);
	}
}