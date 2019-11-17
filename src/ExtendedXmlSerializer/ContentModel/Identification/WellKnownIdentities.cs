using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Identification
{
	sealed class WellKnownIdentities : ReadOnlyDictionary<Assembly, IIdentity>
	{
		public static WellKnownIdentities Default { get; } = new WellKnownIdentities();

		WellKnownIdentities() : base(
		                             new Dictionary<Assembly, IIdentity>
		                             {
			                             {
				                             typeof(IExtendedXmlSerializer).GetTypeInfo()
				                                                           .Assembly,
				                             new Identity("exs", Defaults.Identifier)
			                             },
			                             {
				                             typeof(object).GetTypeInfo()
				                                           .Assembly,
				                             new Identity("sys", "https://extendedxmlserializer.github.io/system")
			                             }
		                             }) {}
	}
}