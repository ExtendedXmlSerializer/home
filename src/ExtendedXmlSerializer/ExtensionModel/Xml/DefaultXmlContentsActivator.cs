using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class DefaultXmlContentsActivator : IXmlContentsActivator
	{
		public static IXmlContentsActivator Default { get; } = new DefaultXmlContentsActivator();

		DefaultXmlContentsActivator() {}

		public IInnerContent Create(IFormatReader reader, object instance, XmlContent content)
			=> new XmlInnerContent(reader, instance, content);
	}
}