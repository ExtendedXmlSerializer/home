using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Options;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Collections
{
	class CollectionOption : ContainerOptionBase
	{
		readonly IConverters _converters;
		readonly ICollectionItemTypeLocator _locator;
		readonly IActivators _activators;
		readonly IAddDelegates _add;

		public CollectionOption(IConverters converters, IElements elements)
			: this(
				converters, CollectionItemTypeLocator.Default, new CollectionItemStartElementProvider(elements), Activators.Default,
				AddDelegates.Default) {}

		public CollectionOption(IConverters converters, ICollectionItemTypeLocator locator, IStartElementProvider providers,
		                        IActivators activators, IAddDelegates add)
			: base(IsCollectionTypeSpecification.Default, providers)
		{
			_converters = converters;
			_locator = locator;
			_activators = activators;
			_add = add;
		}

		protected override IConverter Create(TypeInfo type, IEmitter start)
		{
			var context = new CollectionItem(start, _converters.Get(_locator.Get(type)));
			var activator = new CollectionActivator(new DelegatedActivator(_activators.Get(type.AsType())), context, _add);
			var result = new EnumerableConverter(context, activator);
			return result;
		}
	}
}