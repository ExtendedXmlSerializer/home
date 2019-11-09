using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public interface ISerializerExtension : IAlteration<IServiceRepository>, ICommand<IServices> {}
}