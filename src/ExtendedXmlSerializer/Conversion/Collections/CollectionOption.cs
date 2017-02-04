using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Options;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Collections
{
	class CollectionOption : ConverterOptionBase
	{
		readonly IConverters _converters;
		readonly ICollectionItemTypeLocator _locator;
		readonly IEmitters _emitters;
		readonly IActivators _activators;
		readonly IAddDelegates _add;

		public CollectionOption(IConverters converters)
			: this(
				converters, CollectionItemTypeLocator.Default, Emitters.Default, Activators.Default,
				AddDelegates.Default) {}

		public CollectionOption(IConverters converters, ICollectionItemTypeLocator locator, IEmitters emitters,
		                        IActivators activators, IAddDelegates add)
			: base(IsCollectionTypeSpecification.Default)
		{
			_converters = converters;
			_locator = locator;
			_emitters = emitters;
			_activators = activators;
			_add = add;
		}

		public override IConverter Get(TypeInfo parameter)
		{
			var item = _locator.Get(parameter);
			var context = new CollectionItem(_emitters.Get(item), _converters.Get(item));
			var activator = new CollectionActivator(new DelegatedActivator(_activators.Get(parameter.AsType())), context, _add);
			var result = new EnumerableConverter(context, activator);
			return result;
		}
	}
}