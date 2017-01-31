namespace ExtendedXmlSerialization.Conversion
{
	class DecoratedActivator : IActivator
	{
		readonly IActivator _activator;

		public DecoratedActivator(IActivator activator)
		{
			_activator = activator;
		}

		public virtual object Get(IYielder parameter) => _activator.Get(parameter);
	}
}