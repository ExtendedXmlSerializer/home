using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.Configuration
{
	public interface IRootContext : IContext, IExtensionCollection
	{
		ITypeConfigurations Types { get; }

		IExtendedXmlSerializer Create();
	}
}