using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public interface IServicesFactory : IParameterizedSource<IConfigurationContainer, IServices> {}
}