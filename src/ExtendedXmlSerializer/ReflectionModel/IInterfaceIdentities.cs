using System;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IInterfaceIdentities : IParameterizedSource<TypeInfo, ImmutableArray<Guid>> {}
}