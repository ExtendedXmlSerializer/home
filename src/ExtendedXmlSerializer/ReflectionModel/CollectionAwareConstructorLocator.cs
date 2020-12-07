using ExtendedXmlSerializer.Core.Specifications;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class CollectionAwareConstructorLocator : ListConstructorLocator, IConstructorLocator
	{
		readonly static ISpecification<TypeInfo> Specification = IsInterface.Default.And(IsCollectionType.Instance);

		public CollectionAwareConstructorLocator(IConstructorLocator previous) : base(Specification, previous) {}

		sealed class IsCollectionType : AnySpecification<TypeInfo>
		{
			public static IsCollectionType Instance { get; } = new IsCollectionType();

			IsCollectionType() : base(IsCollectionTypeSpecification.Default,
			                          new IsAssignableGenericSpecification(typeof(IReadOnlyCollection<>))) {}
		}
	}
}