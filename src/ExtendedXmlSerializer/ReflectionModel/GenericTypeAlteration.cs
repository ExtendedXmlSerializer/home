using System;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class GenericTypeAlteration : IParameterizedSource<ImmutableArray<Type>, Type>
	{
		readonly Type _definition;

		public GenericTypeAlteration(Type definition) => _definition = definition;

		public Type Get(ImmutableArray<Type> parameter) => _definition.MakeGenericType(parameter.ToArray());
	}
}