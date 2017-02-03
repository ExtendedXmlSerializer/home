namespace ExtendedXmlSerialization.Conversion.Elements
{
	class ElementEmitter : DecoratedEmitter
	{
		readonly IElement _element;

		public ElementEmitter(IElement element, IEmitter emitter) : base(emitter)
		{
			_element = element;
		}

		public override void Emit(IWriter writer, object instance)
		{
			using (writer.Emit(_element))
			{
				base.Emit(writer, instance);
			}
		}
	}
}