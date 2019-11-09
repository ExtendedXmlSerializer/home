using System;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	public delegate Func<string, ImmutableArray<TypeInfo>?> Partition(string parameter);
}