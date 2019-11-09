using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class ImplicitTypeIdentities : ITypeIdentities
	{
		readonly ITypeIdentities                        _identities;
		readonly IParameterizedSource<string, TypeInfo> _types;

		public ImplicitTypeIdentities(ITypeIdentities identities, IParameterizedSource<string, TypeInfo> types)
		{
			_identities = identities;
			_types      = types;
		}

		public TypeInfo Get(IIdentity parameter) => _identities.Get(parameter) ?? _types.Get(parameter.Name);
	}
}