using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class IsCollectionTypeSpecification : AnySpecification<TypeInfo>
	{
		public static IsCollectionTypeSpecification Default { get; } = new IsCollectionTypeSpecification();

		IsCollectionTypeSpecification()
			: base(IsAssignableSpecification<IList>.Default, new IsAssignableGenericSpecification(typeof(ICollection<>))
			      ) {}
	}
}