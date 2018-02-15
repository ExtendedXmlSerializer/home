using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class NullableValueCoercer<T> : IParameterizedSource<T?, T> where T : struct
	{
		public static NullableValueCoercer<T> Default { get; } = new NullableValueCoercer<T>();
		NullableValueCoercer() {}

		public T Get(T? parameter) => parameter.GetValueOrDefault();
	}

	sealed class InstanceMetadataCoercer<T> : IParameterizedSource<T, TypeInfo>
	{
		public static InstanceMetadataCoercer<T> Default { get; } = new InstanceMetadataCoercer<T>();
		InstanceMetadataCoercer() {}

		public TypeInfo Get(T parameter) => parameter.GetType().GetTypeInfo();
	}

	sealed class InstanceTypeCoercer<T> : IParameterizedSource<T, Type>
	{
		public static InstanceTypeCoercer<T> Default { get; } = new InstanceTypeCoercer<T>();
		InstanceTypeCoercer() { }

		public Type Get(T parameter) => parameter.GetType();
	}
}
