using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class WhitelistReferencesPolicy : ContainsSpecification<TypeInfo>, IReferencesPolicy
	{
		public WhitelistReferencesPolicy(ICollection<TypeInfo> avoid) : base(avoid) {}
	}
}