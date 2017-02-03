namespace ExtendedXmlSerialization.Conversion
{
	class DecoratedEmitter : IEmitter
	{
		readonly IEmitter _emitter;

		public DecoratedEmitter(IEmitter emitter)
		{
			_emitter = emitter;
		}

		public virtual void Emit(IWriter writer, object instance) => _emitter.Emit(writer, instance);
	}
}