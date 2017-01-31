using System.Reflection;
using ExtendedXmlSerialization.Conversion.Model.Names;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion
{
	class CollectionItemNameProvider : NameProviderBase
	{
		readonly INames _names;
		readonly ICollectionItemTypeLocator _locator;

		public CollectionItemNameProvider(ICollectionItemTypeLocator locator, INames names)
			: this(locator, names, new EnumerableTypeFormatter(locator)) {}

		public CollectionItemNameProvider(ICollectionItemTypeLocator locator, INames names, ITypeFormatter formatter)
			: base(formatter)
		{
			_locator = locator;
			_names = names;
		}

		public override IName Create(string displayName, TypeInfo classification) => _names.Get(_locator.Get(classification));
	}
}