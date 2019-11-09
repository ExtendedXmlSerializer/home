using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypeIdentities : TableSource<IIdentity, TypeInfo>, ITypeIdentities
	{
		public TypeIdentities(IDictionary<TypeInfo, string> names, IIdentities identities)
			: base(names.Select(x => x.Key.AsType())
			            .YieldMetadata()
			            .ToDictionary(identities.Get)) {}
	}
}