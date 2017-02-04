using System.Xml;

namespace ExtendedXmlSerialization.Conversion
{
	public class DecoratedConverter : ConverterBase
	{
		readonly IActivator _activator;
		readonly IEmitter _emitter;

		public DecoratedConverter(IConverter converter) : this(converter, converter) {}

		public DecoratedConverter(IActivator activator, IEmitter emitter)
		{
			_activator = activator;
			_emitter = emitter;
		}

		public override void Emit(XmlWriter writer, object instance) => _emitter.Emit(writer, instance);
		public override object Get(IReader reader) => _activator.Get(reader);
	}
}