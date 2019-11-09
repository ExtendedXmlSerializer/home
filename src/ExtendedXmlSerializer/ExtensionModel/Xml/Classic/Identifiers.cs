using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class Identifiers : IIdentifiers
	{
		readonly ITypeIdentification _registration;
		readonly IIdentifiers        _identifiers;

		public Identifiers(ITypeIdentification registration, IIdentifiers identifiers)
		{
			_registration = registration;
			_identifiers  = identifiers;
		}

		public string Get(TypeInfo parameter) => _registration.Get(parameter)
		                                                      ?.Identifier.NullIfEmpty() ??
		                                         _identifiers.Get(parameter);
	}
}