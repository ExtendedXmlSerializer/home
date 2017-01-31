using System.Reflection;
using ExtendedXmlSerialization.Conversion.Model.Names;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion
{
	class CollectionContextOption : ContainerContextOptionBase<IName>
	{
		readonly IContexts _contexts;
		readonly IActivators _activators;
		readonly IAddDelegates _add;

		public CollectionContextOption(IContexts contexts, INameProvider names, IActivators activators,
		                               IAddDelegates add) : base(IsCollectionTypeSpecification.Default, names)
		{
			_contexts = contexts;
			_activators = activators;
			_add = add;
		}

		protected override IElementContext Create(TypeInfo type, IName name)
		{
			var context = new CollectionItemContext(_contexts, name);
			var activator = new CollectionActivator(new DelegatedActivator(_activators.Get(type.AsType())), context, _add);
			var result = new EnumerableContext(context, activator);
			return result;
		}
	}
}