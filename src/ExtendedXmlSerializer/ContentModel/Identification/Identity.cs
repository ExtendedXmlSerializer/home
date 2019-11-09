using System;

namespace ExtendedXmlSerializer.ContentModel.Identification
{
	class Identity : IIdentity, IEquatable<IIdentity>
	{
		readonly static IdentityComparer IdentityComparer = IdentityComparer.Default;
		readonly        int              _code;

		public Identity(string name, string identifier)
		{
			Name       = name;
			Identifier = identifier;
			_code      = IdentityComparer.GetHashCode(this);
		}

		public string Name { get; }
		public string Identifier { get; }

		public static bool operator ==(Identity left, Identity right) => left._code == right._code;

		public static bool operator !=(Identity left, Identity right) => left._code != right._code;

		public bool Equals(IIdentity other) => _code == other.GetHashCode();

		public sealed override int GetHashCode() => _code;

		public sealed override bool Equals(object obj) => obj is IIdentity && Equals((IIdentity)obj);
	}
}