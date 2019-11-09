using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Coercion
{
	sealed class TypeMetadataCoercer : CoercerBase<Type, TypeInfo>
	{
		public static TypeMetadataCoercer Default { get; } = new TypeMetadataCoercer();

		TypeMetadataCoercer() {}

		protected override TypeInfo Get(Type parameter, TypeInfo targetType) => parameter.GetTypeInfo();
	}
}