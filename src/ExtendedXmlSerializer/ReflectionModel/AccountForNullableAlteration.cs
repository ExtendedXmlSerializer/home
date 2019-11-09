using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class AccountForNullableAlteration : IAlteration<TypeInfo>
	{
		public static AccountForNullableAlteration Default { get; } = new AccountForNullableAlteration();

		AccountForNullableAlteration() {}

		public TypeInfo Get(TypeInfo parameter) => Nullable.GetUnderlyingType(parameter.AsType())
		                                                   ?.GetTypeInfo() ?? parameter;
	}
}