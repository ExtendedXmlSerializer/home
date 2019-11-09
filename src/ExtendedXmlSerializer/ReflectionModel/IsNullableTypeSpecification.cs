using System;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class IsNullableTypeSpecification : DelegatedAssignedSpecification<Type, Type>
	{
		public static IsNullableTypeSpecification Default { get; } = new IsNullableTypeSpecification();

		IsNullableTypeSpecification() : base(Nullable.GetUnderlyingType) {}
	}
}