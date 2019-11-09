using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel;

namespace ExtendedXmlSerializer.Configuration
{
	public interface IServicesFactory : IParameterizedSource<IExtensionCollection, IServices> {}
}