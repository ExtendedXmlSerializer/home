using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Options;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Collections
{
	class CollectionOption : ContainerOptionBase
	{
		readonly IConverters _converters;
		readonly IActivators _activators;
		readonly IAddDelegates _add;

		public CollectionOption(IConverters converters, IElementProvider elements, IActivators activators,
		                                 IAddDelegates add) : base(IsCollectionTypeSpecification.Default, elements)
		{
			_converters = converters;
			_activators = activators;
			_add = add;
		}

		protected override IConverter Create(TypeInfo type, IElement element)
		{
			var context = new CollectionItem(element, _converters.Get(element.Classification));
			var activator = new CollectionActivator(new DelegatedActivator(_activators.Get(type.AsType())), context, _add);
			var result = new EnumerableConverter(context, activator);
			return result;
		}
	}
}