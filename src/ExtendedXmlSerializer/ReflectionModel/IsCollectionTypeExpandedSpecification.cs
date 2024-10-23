using ExtendedXmlSerializer.Core.Specifications;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel;

sealed class IsCollectionTypeExpandedSpecification : AnySpecification<TypeInfo>
{
	public static IsCollectionTypeExpandedSpecification Default { get; } = new();

	IsCollectionTypeExpandedSpecification()
		: base(IsCollectionTypeSpecification.Default,
		       new IsAssignableGenericSpecification(typeof(IReadOnlyList<>)),
		       new IsAssignableGenericSpecification(typeof(IReadOnlyCollection<>))) {}
}