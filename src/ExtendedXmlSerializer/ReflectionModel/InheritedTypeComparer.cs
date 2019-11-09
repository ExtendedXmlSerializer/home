using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class InheritedTypeComparer : ITypeComparer
	{
		public static InheritedTypeComparer Default { get; } = new InheritedTypeComparer();

		InheritedTypeComparer() {}

		public bool Equals(TypeInfo x, TypeInfo y) => x.IsAssignableFrom(y);

		public int GetHashCode(TypeInfo obj) => 0;
	}
}