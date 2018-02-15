using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	class TypeMetadataValues<TAttribute, T> : MetadataValues<TypeInfo, TAttribute, T>
		where TAttribute : Attribute, ISource<T> {}

	class MetadataValues<TAttribute, T> : MetadataValues<MemberInfo, TAttribute, T>
		where TAttribute : Attribute, ISource<T> {}

	class MetadataValues<TMember, TAttribute, T> : IParameterizedSource<TMember, ImmutableArray<T>>
		where TAttribute : Attribute, ISource<T>
		where TMember : MemberInfo
	{
		public ImmutableArray<T> Get(TMember parameter) => parameter.GetCustomAttributes<TAttribute>()
		                                                            .Select(x => x.Get())
		                                                            .ToImmutableArray();
	}

	class TypeMetadataValue<TAttribute, T> : MetadataValue<TypeInfo, TAttribute, T>
		where TAttribute : Attribute, ISource<T> {}

	class MetadataValue<TAttribute, T> : MetadataValue<MemberInfo, TAttribute, T>
		where TAttribute : Attribute, ISource<T> {}

	class MetadataValue<TMember, TAttribute, T> : IParameterizedSource<TMember, T>
		where TAttribute : Attribute, ISource<T>
		where TMember : MemberInfo
	{
		readonly ISpecification<TMember> _specification;

		public MetadataValue() : this(IsDefinedSpecification<TAttribute>.Default) {}
		public MetadataValue(ISpecification<TMember> specification) => _specification = specification;

		public T Get(TMember parameter)
			=> _specification.IsSatisfiedBy(parameter)
				   ? parameter.GetCustomAttribute<TAttribute>()
				              .Get()
				   : default(T);
	}
}
