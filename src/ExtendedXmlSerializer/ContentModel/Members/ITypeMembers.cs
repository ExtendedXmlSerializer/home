using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	interface ITypeMembers : IParameterizedSource<TypeInfo, ImmutableArray<IMember>> {}
}