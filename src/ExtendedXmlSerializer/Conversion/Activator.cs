using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion
{
	class Activator : IActivator
	{
		readonly IActivators _activators;

		public Activator(IActivators activators)
		{
			_activators = activators;
		}

		public object Get(IReader parameter) => _activators.Get(parameter.Classification.AsType()).Invoke();
	}
}