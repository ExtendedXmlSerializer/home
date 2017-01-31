using System.Collections;
using System.Reflection;

namespace ExtendedXmlSerialization.Conversion.Collections
{
	class EnumerableConverter : EnumerableConverter<IEnumerable>
	{
		public EnumerableConverter(IConverter item, IActivator activator) : base(item, activator) {}
	}

	class EnumerableConverter<T> : ConverterBase<T> where T : IEnumerable
	{
		readonly IConverter _item;
		readonly IActivator _activator;

		public EnumerableConverter(IConverter item, IActivator activator) : this(item, activator, item.Classification) {}

		public EnumerableConverter(IConverter item, IActivator activator, TypeInfo classification) : base(classification)
		{
			_item = item;
			_activator = activator;
		}

		protected virtual IEnumerator Get(T instance) => instance.GetEnumerator();

		public override void Emit(IWriter writer, T instance)
		{
			var enumerator = Get(instance);
			while (enumerator.MoveNext())
			{
				_item.Write(writer, enumerator.Current);
			}
		}

		public override object Get(IReader reader) => _activator.Get(reader);
	}
}