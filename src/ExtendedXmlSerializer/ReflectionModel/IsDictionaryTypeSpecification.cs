using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class IsDictionaryTypeSpecification : AnySpecification<TypeInfo>
	{
		public static IsDictionaryTypeSpecification Default { get; } = new IsDictionaryTypeSpecification();

		IsDictionaryTypeSpecification()
			: base(
			       IsAssignableSpecification<IDictionary>.Default,
			       new IsAssignableGenericSpecification(typeof(IDictionary<,>))) {}
	}
}