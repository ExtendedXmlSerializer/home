using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class MappedArraySpecification : ISpecification<TypeInfo>
	{
		public static MappedArraySpecification Default { get; } = new MappedArraySpecification();

		readonly ISpecification<TypeInfo> _specification;

		public MappedArraySpecification() : this(IsValueTypeSpecification.Default) {}

		public MappedArraySpecification(ISpecification<TypeInfo> specification)
		{
			_specification = specification;
		}

		public bool IsSatisfiedBy(TypeInfo parameter) => parameter.GetArrayRank() > 1 &&
		                                                 _specification.IsSatisfiedBy(parameter.GetElementType());
	}
}