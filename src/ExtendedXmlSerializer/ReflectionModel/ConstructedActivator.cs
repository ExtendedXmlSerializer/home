using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ConstructedActivator : IActivator
	{
		readonly ConstructorInfo     _constructor;
		readonly IEnumerable<object> _arguments;

		public ConstructedActivator(ConstructorInfo constructor, IEnumerable<object> arguments)
		{
			_constructor = constructor;
			_arguments   = arguments;
		}

		public object Get() => _constructor.Invoke(_arguments.ToArray());
	}
}