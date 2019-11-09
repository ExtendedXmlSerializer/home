using System.Collections;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlListContentsActivator : IXmlContentsActivator
	{
		public static XmlListContentsActivator Default { get; } = new XmlListContentsActivator();

		XmlListContentsActivator() : this(Lists.Default) {}

		readonly ILists _lists;

		public XmlListContentsActivator(ILists lists) => _lists = lists;

		public IInnerContent Create(IFormatReader reader, object instance, XmlContent content)
			=> new XmlListInnerContent(reader, instance, instance as IList ?? _lists.Get(instance), content);
	}
}