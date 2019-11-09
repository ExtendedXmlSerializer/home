using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Identification
{
	sealed class Aliases : TableSource<string, string>, IAliases
	{
		public static Aliases Default { get; } = new Aliases();

		Aliases() : this(WellKnownIdentities.Default) {}

		public Aliases(IDictionary<Assembly, IIdentity> known)
			: base(known.Values.ToDictionary(x => x.Identifier, x => x.Name)) {}
	}
}