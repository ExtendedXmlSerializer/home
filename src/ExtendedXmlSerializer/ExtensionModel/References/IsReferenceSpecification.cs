using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class IsReferenceSpecification : InverseSpecification<TypeInfo>
	{
		public static IsReferenceSpecification Default { get; } = new IsReferenceSpecification();

		IsReferenceSpecification() : base(IsValueTypeSpecification.Default) {}
	}
}