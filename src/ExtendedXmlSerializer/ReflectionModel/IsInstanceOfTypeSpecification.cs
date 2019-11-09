using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class IsInstanceOfTypeSpecification<T> : DelegatedSpecification<object>
	{
		public static IsInstanceOfTypeSpecification<T> Default { get; } = new IsInstanceOfTypeSpecification<T>();

		IsInstanceOfTypeSpecification() : base(IsInstanceOfTypeSpecification.Delegates.Get(typeof(T).GetTypeInfo())) {}
	}

	sealed class IsInstanceOfTypeSpecification : DelegatedSpecification<object>
	{
		public static IParameterizedSource<TypeInfo, ISpecification<object>> Defaults { get; } =
			new ReferenceCache<TypeInfo, ISpecification<object>>(x => new IsInstanceOfTypeSpecification(x));

		public static IParameterizedSource<TypeInfo, Func<object, bool>> Delegates { get; } =
			new ReferenceCache<TypeInfo, Func<object, bool>>(x => Defaults.Get(x)
			                                                              .IsSatisfiedBy);

		IsInstanceOfTypeSpecification(TypeInfo type) : base(type.IsInstanceOfType) {}
	}
}