using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	public sealed class IsAssignableSpecification<T> : DelegatedSpecification<TypeInfo>
	{
		public static IsAssignableSpecification<T> Default { get; } = new IsAssignableSpecification<T>();

		IsAssignableSpecification() : base(IsAssignableSpecification.Delegates.Get(typeof(T).GetTypeInfo())) {}
	}

	public sealed class IsAssignableSpecification : DelegatedSpecification<TypeInfo>
	{
		public static IParameterizedSource<TypeInfo, ISpecification<TypeInfo>> Defaults { get; } =
			new ReferenceCache<TypeInfo, ISpecification<TypeInfo>>(x => new IsAssignableSpecification(x));

		public static IParameterizedSource<TypeInfo, Func<TypeInfo, bool>> Delegates { get; } =
			new ReferenceCache<TypeInfo, Func<TypeInfo, bool>>(x => Defaults.Get(x)
			                                                                .IsSatisfiedBy);

		IsAssignableSpecification(TypeInfo type) : base(type.IsAssignableFrom) {}
	}
}