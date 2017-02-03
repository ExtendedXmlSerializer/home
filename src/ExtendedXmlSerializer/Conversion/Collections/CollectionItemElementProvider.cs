using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Collections
{
	class CollectionItemElementProvider : ElementProviderBase
	{
		readonly IElements _elements;
		readonly ICollectionItemTypeLocator _locator;

		public CollectionItemElementProvider(IElements elements, IAliasProvider alias)
			: this(elements, CollectionItemTypeLocator.Default, alias) {}

		public CollectionItemElementProvider(IElements elements, ICollectionItemTypeLocator locator, IAliasProvider @alias)
			: base(alias, TypeFormatter.Default)
		{
			_locator = locator;
			_elements = elements;
		}

		public override IElement Create(string displayName, TypeInfo classification)
			=> _elements.Get(_locator.Get(classification));
	}
}