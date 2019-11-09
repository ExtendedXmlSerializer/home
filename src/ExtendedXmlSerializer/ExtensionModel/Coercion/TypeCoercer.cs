using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Coercion
{
	sealed class TypeCoercer : CoercerBase<TypeInfo, Type>
	{
		public static TypeCoercer Default { get; } = new TypeCoercer();

		TypeCoercer() {}

		protected override Type Get(TypeInfo parameter, TypeInfo targetType) => parameter.AsType();
	}
}