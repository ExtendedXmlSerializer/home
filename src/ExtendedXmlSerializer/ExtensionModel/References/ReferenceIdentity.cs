using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	readonly struct ReferenceIdentity : IEquatable<ReferenceIdentity>
	{
		readonly static TypeInfo Reference = Defaults.Reference;

		readonly int _code;

		public ReferenceIdentity(object identifier) : this(Reference, identifier) {}

		public ReferenceIdentity(TypeInfo type, object identifier)
			: this((type.GetHashCode() * 397) ^ identifier.GetHashCode()) {}

		public ReferenceIdentity(uint identifier) : this(Reference, identifier) {}

		public ReferenceIdentity(TypeInfo type, uint identifier) : this((type.GetHashCode() * 397) ^
		                                                                identifier.GetHashCode()) {}

		ReferenceIdentity(int code) => _code = code;

		public bool Equals(ReferenceIdentity other)
		{
			var code   = _code;
			var result = code.Equals(other._code);
			return result;
		}

		public override bool Equals(object obj)
			=> !ReferenceEquals(null, obj) && obj is ReferenceIdentity && Equals((ReferenceIdentity)obj);

		public override int GetHashCode() => _code;

		public static bool operator ==(ReferenceIdentity left, ReferenceIdentity right) => left.Equals(right);

		public static bool operator !=(ReferenceIdentity left, ReferenceIdentity right) => !left.Equals(right);
	}
}