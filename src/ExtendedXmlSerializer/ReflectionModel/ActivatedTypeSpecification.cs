using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ActivatedTypeSpecification : ISpecification<TypeInfo>
	{
		readonly static TypeInfo GeneralObject = typeof(object).GetTypeInfo();

		public static ActivatedTypeSpecification Default { get; } = new ActivatedTypeSpecification();

		ActivatedTypeSpecification() {}

		public bool IsSatisfiedBy(TypeInfo parameter)
			=> !parameter.IsAbstract && parameter.IsClass && !parameter.Equals(GeneralObject);
	}
}