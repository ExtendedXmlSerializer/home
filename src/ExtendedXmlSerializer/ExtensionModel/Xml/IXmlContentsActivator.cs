using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	interface IXmlContentsActivator
	{
		IInnerContent Create(IFormatReader reader, object instance, XmlContent content);
	}
}