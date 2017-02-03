using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Collections
{
	class CollectionItemElementProvider : ElementProviderBase
	{
		readonly IElements _elements;
		readonly ICollectionItemTypeLocator _locator;

		public CollectionItemElementProvider(IElements elements) : this(CollectionItemTypeLocator.Default, elements) {}

		public CollectionItemElementProvider(ICollectionItemTypeLocator locator, IElements elements)
			: base(new EnumerableTypeFormatter(locator))
		{
			_locator = locator;
			_elements = elements;
		}

		public override IElement Create(string displayName, TypeInfo classification) => _elements.Get(_locator.Get(classification));
	}
}