using ExtendedXmlSerializer.Core.Specifications;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class BlacklistReferencesPolicy : InverseSpecification<TypeInfo>, IReferencesPolicy
	{
		public BlacklistReferencesPolicy(params TypeInfo[] avoid) : this(new HashSet<TypeInfo>(avoid)) {}

		public BlacklistReferencesPolicy(ICollection<TypeInfo> avoid)
			: base(new ContainsSpecification<TypeInfo>(avoid)) {}
	}
}