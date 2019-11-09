using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class DefaultNames : ReadOnlyDictionary<TypeInfo, string>
	{
		public static DefaultNames Default { get; } = new DefaultNames();

		DefaultNames() : base(WellKnownAliases.Default.ToDictionary(x => x.Key.GetTypeInfo(), x => x.Value)) {}
	}
}