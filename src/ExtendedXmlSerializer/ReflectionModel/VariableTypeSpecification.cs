using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class VariableTypeSpecification : InverseSpecification<TypeInfo>
	{
		public static VariableTypeSpecification Default { get; } = new VariableTypeSpecification();

		VariableTypeSpecification() : base(FixedTypeSpecification.Default) {}
	}
}