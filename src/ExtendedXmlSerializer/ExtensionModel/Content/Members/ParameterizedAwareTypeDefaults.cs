using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.ExtensionModel.Types;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ParameterizedAwareTypeDefaults : ITypeDefaults
	{
		readonly ITypeDefaults        _previous;
		readonly IQueriedConstructors _constructors;
		readonly ITypeDefaults        _defaults;

		public ParameterizedAwareTypeDefaults(ITypeDefaults previous, IQueriedConstructors constructors)
			: this(previous, constructors, DefaultValueTypeDefaults.Default) {}

		public ParameterizedAwareTypeDefaults(ITypeDefaults previous, IQueriedConstructors constructors,
		                                      ITypeDefaults defaults)
		{
			_previous     = previous;
			_constructors = constructors;
			_defaults     = defaults;
		}

		public object Get(TypeInfo parameter)
		{
			var defaults = _constructors.Get(parameter) != null ? _defaults : _previous;
			var result   = defaults.Get(parameter);
			return result;
		}
	}
}