using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class ImplicitIdentities : IIdentities
	{
		readonly ImmutableArray<TypeInfo> _implicit;
		readonly IIdentities              _identities;

		public ImplicitIdentities(ImmutableArray<TypeInfo> @implicit, IIdentities identities)
		{
			_implicit   = @implicit;
			_identities = identities;
		}

		public IIdentity Get(TypeInfo parameter)
		{
			var identity = _identities.Get(parameter);
			var array    = _implicit;
			var result   = array.Contains(parameter) ? new Identity(identity.Name, null) : identity;
			return result;
		}
	}
}