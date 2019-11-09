using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class AlteringActivator : IActivator
	{
		readonly IAlteration<object> _alteration;
		readonly IActivator          _activator;

		public AlteringActivator(IAlteration<object> alteration, IActivator activator)
		{
			_alteration = alteration;
			_activator  = activator;
		}

		public object Get() => _alteration.Get(_activator.Get());
	}
}