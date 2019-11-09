using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class IsGenericDictionarySpecification : AllSpecification<TypeInfo>
	{
		public static IsGenericDictionarySpecification Default { get; } = new IsGenericDictionarySpecification();

		IsGenericDictionarySpecification() : base(IsGenericTypeSpecification.Default,
		                                          new IsGenericDefinitionSpecification(typeof(Dictionary<,>))) {}
	}
}