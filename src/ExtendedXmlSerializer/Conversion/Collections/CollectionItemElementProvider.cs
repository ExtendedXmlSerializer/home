using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Collections
{
	class CollectionItemElementProvider : ElementProviderBase
	{
		readonly IElements _elements;
		readonly ICollectionItemTypeLocator _locator;

		public CollectionItemElementProvider(ICollectionItemTypeLocator locator, IElements elements)
			: this(locator, elements, new EnumerableTypeFormatter(locator)) {}

		public CollectionItemElementProvider(ICollectionItemTypeLocator locator, IElements elements, ITypeFormatter formatter)
			: base(formatter)
		{
			_locator = locator;
			_elements = elements;
		}

		public override IElement Create(string displayName, TypeInfo classification) => _elements.Get(_locator.Get(classification));
	}
}