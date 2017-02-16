using System.Collections.Generic;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	public class IdentityComparer : IdentityComparer<IIdentity>
	{
		public new static IdentityComparer Default { get; } = new IdentityComparer();
		IdentityComparer() {}
	}

	public class IdentityComparer<T> : IEqualityComparer<T> where T : IIdentity
	{
		public static IdentityComparer<T> Default { get; } = new IdentityComparer<T>();
		protected IdentityComparer() {}

		public bool Equals(T x, T y)
			=> ReferenceEquals(x, y) || string.Equals(x.Name, y.Name) && string.Equals(x.Identifier, y.Identifier);

		public int GetHashCode(T obj) => obj.Name.GetHashCode() ^ obj.Identifier.GetHashCode();
	}
}