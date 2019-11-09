using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlContentsActivatorSelector : DecoratedSource<TypeInfo, IXmlContentsActivator>
	{
		public static XmlContentsActivatorSelector Default { get; } = new XmlContentsActivatorSelector();

		XmlContentsActivatorSelector()
			: this(XmlListContentsActivator.Default,
			       new XmlListContentsActivator(new Lists(DictionaryAddDelegates.Default))) {}

		public XmlContentsActivatorSelector(IXmlContentsActivator collection, IXmlContentsActivator dictionary)
			: base(DefaultXmlContentsActivator.Default
			                                  .Let(IsCollectionTypeSpecification.Default, collection)
			                                  .Let(IsDictionaryTypeSpecification.Default, dictionary)) {}
	}
}