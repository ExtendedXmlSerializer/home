using ExtendedXmlSerializer.Core.Specifications;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class WhitelistReferencesPolicy : ContainsSpecification<TypeInfo>, IReferencesPolicy
	{
		public WhitelistReferencesPolicy(ICollection<TypeInfo> allow) : base(allow) {}
	}
}