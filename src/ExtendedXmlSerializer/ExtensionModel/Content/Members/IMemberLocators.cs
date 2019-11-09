using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	interface IMemberLocators : IParameterizedSource<TypeInfo, IMetadataLocator> {}
}