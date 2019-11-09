using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class TypeCoercer : IParameterizedSource<TypeInfo, Type>
	{
		public static TypeCoercer Default { get; } = new TypeCoercer();

		TypeCoercer() {}

		public Type Get(TypeInfo parameter) => parameter.AsType();
	}
}