using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class TypeMetadataCoercer : IParameterizedSource<Type, TypeInfo>
	{
		public static TypeMetadataCoercer Default { get; } = new TypeMetadataCoercer();

		TypeMetadataCoercer() {}

		public TypeInfo Get(Type parameter) => parameter.GetTypeInfo();
	}
}