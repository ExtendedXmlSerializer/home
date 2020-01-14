using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ContentModel.Identification
{
	sealed class Identifiers : IIdentifiers
	{
		readonly IReadOnlyDictionary<Assembly, IIdentity> _known;
		readonly INamespaceFormatter                      _formatter;

		public Identifiers(IReadOnlyDictionary<Assembly, IIdentity> known, INamespaceFormatter formatter)
		{
			_known     = known;
			_formatter = formatter;
		}

		public string Get(TypeInfo parameter)
			=> _known.Get(parameter.Assembly)?.Identifier ?? _formatter.Get(parameter);
	}
}