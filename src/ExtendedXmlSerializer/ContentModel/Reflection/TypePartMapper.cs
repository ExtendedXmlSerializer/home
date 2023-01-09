using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypePartMapper : IAlteration<TypeParts>
	{
		readonly IIdentityStore             _store;
		readonly Func<TypeParts, TypeParts> _selector;

		public TypePartMapper(IIdentityStore store)
		{
			_store    = store;
			_selector = Get;
		}

		public TypeParts Get(TypeParts parameter)
		{
			var arguments = parameter.GetArguments();
			var identity  = _store.Get(parameter.Name, parameter.Identifier);
			var result = new TypeParts(identity.Name, identity.Identifier,
			                           arguments.HasValue
				                           ? arguments.Value.Select(_selector)
				                                      .ToImmutableArray
				                           : null);
			return result;
		}
	}
}