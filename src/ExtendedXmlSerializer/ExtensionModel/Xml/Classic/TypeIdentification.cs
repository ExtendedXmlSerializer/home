using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class TypeIdentification : ITypeIdentification
	{
		readonly IParameterizedSource<TypeInfo, IIdentity> _identities;
		readonly IParameterizedSource<IIdentity, TypeInfo> _types;

		public TypeIdentification(IDictionary<TypeInfo, IIdentity> store)
			: this(new TableSource<TypeInfo, IIdentity>(store),
			       new TableSource<IIdentity, TypeInfo>(store.ToDictionary(x => x.Value, x => x.Key))) {}

		public TypeIdentification(IParameterizedSource<TypeInfo, IIdentity> identities,
		                          IParameterizedSource<IIdentity, TypeInfo> types)
		{
			_identities = identities;
			_types      = types;
		}

		public IIdentity Get(TypeInfo parameter) => _identities.Get(parameter);

		public TypeInfo Get(IIdentity parameter) => _types.Get(parameter);
	}
}