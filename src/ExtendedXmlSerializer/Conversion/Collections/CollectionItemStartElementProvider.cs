using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Collections
{
	class CollectionItemStartElementProvider : StartElementProviderBase
	{
		readonly IElements _elements;
		readonly ICollectionItemTypeLocator _locator;

		public CollectionItemStartElementProvider(IElements elements) : this(elements, CollectionItemTypeLocator.Default) {}

		public CollectionItemStartElementProvider(IElements elements, ICollectionItemTypeLocator locator)
		{
			_locator = locator;
			_elements = elements;
		}

		public override IEmitter Create(string displayName, TypeInfo classification, string @namespace)
			=> _elements.Get(_locator.Get(classification));
	}
}