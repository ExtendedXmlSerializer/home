using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class TypeComparer<T> : IEqualityComparer<T>
	{
		public static TypeComparer<T> Default { get; } = new TypeComparer<T>();

		TypeComparer() {}

		public bool Equals(T x, T y) => x.GetType() == y.GetType();

		public int GetHashCode(T obj) => 0;
	}
}