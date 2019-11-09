using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ActivationContextActivator : IActivator
	{
		readonly IActivationContexts                          _contexts;
		readonly ImmutableArray<KeyValuePair<string, object>> _defaults;

		public ActivationContextActivator(IActivationContexts contexts,
		                                  ImmutableArray<KeyValuePair<string, object>> defaults)
		{
			_contexts = contexts;
			_defaults = defaults;
		}

		public object Get()
			=> _contexts.Get(new Dictionary<string, object>(_defaults.ToDictionary(),
			                                                StringComparer.OrdinalIgnoreCase));
	}
}