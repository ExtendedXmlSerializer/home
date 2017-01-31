using System.Collections;
using System.Reflection;

namespace ExtendedXmlSerialization.Conversion
{
	class EnumerableContext : EnumerableContext<IEnumerable>
	{
		public EnumerableContext(IElementContext item, IActivator activator) : base(item, activator) {}
	}

	class EnumerableContext<T> : ContextBase<T> where T : IEnumerable
	{
		readonly IElementContext _item;
		readonly IActivator _activator;

		public EnumerableContext(IElementContext item, IActivator activator) : this(item, activator, item.Classification) {}

		public EnumerableContext(IElementContext item, IActivator activator, TypeInfo classification) : base(classification)
		{
			_item = item;
			_activator = activator;
		}

		protected virtual IEnumerator Get(T instance) => instance.GetEnumerator();

		public override void Emit(IEmitter emitter, T instance)
		{
			var enumerator = Get(instance);
			while (enumerator.MoveNext())
			{
				_item.Emit(emitter, enumerator.Current);
			}
		}

		public override object Yield(IYielder yielder) => _activator.Get(yielder);
	}
}