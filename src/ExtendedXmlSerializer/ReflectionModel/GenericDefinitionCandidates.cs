using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ReflectionModel
{
	class GenericDefinitionCandidates : IGenericDefinitionCandidates
	{
		public static GenericDefinitionCandidates Default { get; } = new GenericDefinitionCandidates();

		GenericDefinitionCandidates() {}

		// ATTRIBUTION: http://stackoverflow.com/a/5461399/3602057
		public ImmutableArray<Type> Get(TypeInfo parameter) => parameter.AsType()
		                                                                .Yield()
		                                                                .Appending(parameter.GetInterfaces())
		                                                                .YieldMetadata()
		                                                                .Where(x => x.IsGenericType)
		                                                                .Select(x => x.GetGenericTypeDefinition())
		                                                                .ToImmutableArray();
	}
}