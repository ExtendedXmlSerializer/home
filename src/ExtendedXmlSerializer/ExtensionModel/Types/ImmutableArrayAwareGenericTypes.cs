using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ImmutableArrayAwareGenericTypes : IGenericTypes
	{
		readonly static TypeInfo Check = typeof(ImmutableArray).GetTypeInfo();
		readonly static ImmutableArray<TypeInfo> Type = typeof(ImmutableArray<>).GetTypeInfo()
		                                                                        .Yield()
		                                                                        .ToImmutableArray();

		readonly IGenericTypes _types;

		public ImmutableArrayAwareGenericTypes(IGenericTypes types)
		{
			_types = types;
		}

		public ImmutableArray<TypeInfo> Get(IIdentity parameter)
		{
			var type   = _types.Get(parameter);
			var result = Equals(type.Only(), Check) ? Type : type;
			return result;
		}
	}
}