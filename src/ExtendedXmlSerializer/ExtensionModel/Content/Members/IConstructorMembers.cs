using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	interface IConstructorMembers : IParameterizedSource<ConstructorInfo, ImmutableArray<IMember>?> {}
}