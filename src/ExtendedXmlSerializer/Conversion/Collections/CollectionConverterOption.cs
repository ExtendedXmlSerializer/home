using System.Reflection;
using ExtendedXmlSerialization.Conversion.Names;
using ExtendedXmlSerialization.Conversion.Options;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Collections
{
	class CollectionConverterOption : ContainerConverterOptionBase<IName>
	{
		readonly IConverters _converters;
		readonly IActivators _activators;
		readonly IAddDelegates _add;

		public CollectionConverterOption(IConverters converters, INameProvider names, IActivators activators,
		                                 IAddDelegates add) : base(IsCollectionTypeSpecification.Default, names)
		{
			_converters = converters;
			_activators = activators;
			_add = add;
		}

		protected override IConverter Create(TypeInfo type, IName name)
		{
			var context = new CollectionItem(name, _converters.Get(name.Classification));
			var activator = new CollectionActivator(new DelegatedActivator(_activators.Get(type.AsType())), context, _add);
			var result = new EnumerableConverter(context, activator);
			return result;
		}
	}
}