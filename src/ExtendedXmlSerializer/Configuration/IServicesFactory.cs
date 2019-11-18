using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel;

namespace ExtendedXmlSerializer.Configuration
{
	interface IServicesFactory : IParameterizedSource<IExtensionCollection, IServices> {}
}