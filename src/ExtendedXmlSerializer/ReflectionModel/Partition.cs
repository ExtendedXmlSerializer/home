using System;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	delegate Func<string, ImmutableArray<TypeInfo>?> Partition(string parameter);
}