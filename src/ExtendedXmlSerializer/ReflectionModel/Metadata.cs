using ExtendedXmlSerializer.Core.Sources;
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

	class MetadataValues<TMember, TAttribute, T> : SpecificationSource<TMember, ImmutableArray<T>>
		where TAttribute : Attribute, ISource<T>
		where TMember : MemberInfo
	{
		public MetadataValues() : base(IsDefinedSpecification<TAttribute>.Default, Attributes<TAttribute, T>.Default) {}
	}

	class InstanceMetadata<TAttribute, TInstance, TValue> : SpecificationSource<TInstance, TValue>
		where TAttribute : Attribute, ISource<TValue>
		where TValue : class 
	{
		public InstanceMetadata() : this(new TypeMetadataValue<TAttribute, TValue>().ReferenceCache().In(InstanceMetadataCoercer<TInstance>.Default)) {}

		public InstanceMetadata(ISpecificationSource<TInstance, TValue> source) : base(source, source) {}
	}

	class InstanceMetadataValue<TAttribute, TInstance, TValue> : SpecificationSource<TInstance, TValue>
		where TAttribute : Attribute, ISource<TValue>
		where TValue : struct
	{
		public InstanceMetadataValue() : this(new TypeMetadataValue<TAttribute, TValue>().StructureCache().In(InstanceMetadataCoercer<TInstance>.Default)) { }

		public InstanceMetadataValue(ISpecificationSource<TInstance, TValue> source) : base(source, source) { }
	}

	class TypeMetadataValue<TAttribute, T> : MetadataValue<TypeInfo, TAttribute, T>
		where TAttribute : Attribute, ISource<T> {}

	class MetadataValue<TAttribute, T> : MetadataValue<MemberInfo, TAttribute, T>
		where TAttribute : Attribute, ISource<T> {}

	class MetadataValue<TMember, TAttribute, T> : SpecificationSource<TMember, T>
		where TAttribute : Attribute, ISource<T>
		where TMember : MemberInfo
	{
		public MetadataValue() : base(IsDefinedSpecification<TAttribute>.Default, Attribute<TAttribute, T>.Default) {}
	}

	sealed class Attributes<TAttribute, T> : IParameterizedSource<MemberInfo, ImmutableArray<T>>
		where TAttribute : Attribute, ISource<T>
	{
		public static Attributes<TAttribute, T> Default { get; } = new Attributes<TAttribute, T>();
		Attributes() {}

		public ImmutableArray<T> Get(MemberInfo parameter) => parameter.GetCustomAttributes<TAttribute>()
		                                                               .Select(x => x.Get())
		                                                               .ToImmutableArray();
	}

	sealed class Attribute<TAttribute, T> : IParameterizedSource<MemberInfo, T> where TAttribute : Attribute, ISource<T>
	{
		public static Attribute<TAttribute, T> Default { get; } = new Attribute<TAttribute, T>();
		Attribute() {}

		public T Get(MemberInfo parameter) => parameter.GetCustomAttribute<TAttribute>()
		                                               .Get();
	}
}
