namespace ExtendedXmlSerialization.Conversion
{
	class DecoratedActivator : IActivator
	{
		readonly IActivator _activator;

		public DecoratedActivator(IActivator activator)
		{
			_activator = activator;
		}

		public virtual object Get(IReader parameter) => _activator.Get(parameter);
	}

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