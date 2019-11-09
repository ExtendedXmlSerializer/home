using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ImplementedTypeComparer : ITypeComparer
	{
		public static ImplementedTypeComparer Default { get; } = new ImplementedTypeComparer();

		ImplementedTypeComparer() : this(InterfaceIdentities.Default, TypeDefinitionIdentityComparer.Default) {}

		readonly IInterfaceIdentities _interfaces;
		readonly ITypeComparer        _identity;

		public ImplementedTypeComparer(IInterfaceIdentities interfaces, ITypeComparer identity)
		{
			_interfaces = interfaces;
			_identity   = identity;
		}

		public bool Equals(TypeInfo x, TypeInfo y)
		{
			var left = x.IsInterface;
			if (left != y.IsInterface)
			{
				var @interface     = left ? x : y;
				var implementation = left ? y : x;
				var contains = _interfaces.Get(implementation)
				                          .Contains(@interface.GUID);
				return contains;
			}

			var result = _identity.Equals(x, y);
			return result;
		}

		public int GetHashCode(TypeInfo obj) => 0;
	}
}