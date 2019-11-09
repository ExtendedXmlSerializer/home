using System;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IGenericDefinitionCandidates : IParameterizedSource<TypeInfo, ImmutableArray<Type>> {}
}