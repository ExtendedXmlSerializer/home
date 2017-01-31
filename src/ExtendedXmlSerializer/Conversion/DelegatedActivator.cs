using System;

namespace ExtendedXmlSerialization.Conversion
{
	class DelegatedActivator : IActivator
	{
		readonly Func<object> _activate;

		public DelegatedActivator(Func<object> activate)
		{
			_activate = activate;
		}

		public object Get(IYielder parameter) => _activate();
	}
}