using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Content.Runtime;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Reflection;
using IServiceProvider = System.IServiceProvider;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class RuntimePropertyContentExtension : ISerializerExtension,
	                                               IAssignable<MemberInfo, IService<ISpecification<object>>>,
	                                               ICommand<MemberInfo>
	{
		readonly IMetadataTable<ISpecification<object>> _specifications;

		[UsedImplicitly]
		public RuntimePropertyContentExtension() : this(new RuntimePropertySpecifications()) { }

		public RuntimePropertyContentExtension(IMetadataTable<ISpecification<object>> specifications) => _specifications = specifications;

		public void Execute(KeyValuePair<MemberInfo, IService<ISpecification<object>>> parameter)
		{
			_specifications.Execute(parameter);
		}

		public void Execute(MemberInfo parameter)
		{
			_specifications.Remove(parameter);
		}

		public IServiceRepository Get(IServiceRepository parameter)
			=> _specifications.Get(parameter)
			                  .RegisterDefinition<IRuntimePropertySpecifications<object>, RuntimePropertySpecifications<object>>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}



	interface IRuntimePropertySpecifications : IMetadata<ISpecification<object>> { }

	sealed class RuntimePropertySpecifications : MetadataTable<ISpecification<object>, IRuntimePropertySpecifications>, IRuntimePropertySpecifications { }

	interface IRuntimePropertySpecifications<T> : ISpecificationSource<MemberInfo, ISpecification<Writing<T>>> { }

	sealed class RuntimePropertySpecifications<T> : ServiceContainer<MemberInfo, ISpecification<Writing<T>>>, IRuntimePropertySpecifications<T>
	{
		public RuntimePropertySpecifications(IServiceProvider provider, IRuntimePropertySpecifications table)
			: base(provider, table.To(A<IService<ISpecification<Writing<T>>>>.Default)) { }
	}


	sealed class RuntimePropertyOrderAlteration<T> : EnumerableAlterations<IMemberContentWriter<T>>, IMemberContentsAlteration<T>
	{
		public RuntimePropertyOrderAlteration(IMemberContentsAlteration<T> alteration)
			: this(IsRuntimePropertySpecification.Default.To(A<IMemberContentWriter<T>>.Default), alteration) {}

		public RuntimePropertyOrderAlteration(ISpecification<IMemberContentWriter<T>> specification, IMemberContentsAlteration<T> alteration)
			: base(new OrderByAlteration<IMemberContentWriter<T>, bool>(specification.IsSatisfiedBy),
			       alteration) {}
	}

	sealed class IsRuntimePropertySpecification : ProxySpecification<object>
	{
		public static IsRuntimePropertySpecification Default { get; } = new IsRuntimePropertySpecification();
		IsRuntimePropertySpecification() : base(RuntimePropertyToken.Default) {}
	}

	sealed class RuntimePropertyToken
	{
		public static RuntimePropertyToken Default { get; } = new RuntimePropertyToken();
		RuntimePropertyToken() {}
	}

	sealed class PropertyTokenSpecification : EqualitySpecification<object>
	{
		public static PropertyTokenSpecification Default { get; } = new PropertyTokenSpecification();
		PropertyTokenSpecification() : base(RuntimePropertyToken.Default) { }
	}

	sealed class RuntimePropertyPipelineComposer<T> : IRuntimePipelineComposer<T>
	{
		readonly ISpecification<object> _specification;
		readonly ISpecificationSource<IMember, ISpecification<Writing<T>>> _specifications;
		readonly IRuntimeMemberWriters<T> _writers;

		public RuntimePropertyPipelineComposer(IRuntimePropertySpecifications<T> specifications, IRuntimeMemberWriters<T> writers)
			: this(PropertyTokenSpecification.Default, specifications.In(MemberMetadataCoercer.Default), writers) { }

		public RuntimePropertyPipelineComposer(ISpecification<object> specification,
		                                       ISpecificationSource<IMember, ISpecification<Writing<T>>> specifications,
		                                       IRuntimeMemberWriters<T> writers)
		{
			_specification = specification;
			_specifications = specifications;
			_writers = writers;
		}

		public IRuntimePipelinePart<T> Get(IRuntimePipelinePart<T> parameter)
		{
			var member = parameter.Get();
			if (_specifications.IsSatisfiedBy(member))
			{
				var specification = _specifications.Get(member);
				var property = _writers.Get(member);
				var condition = new ConditionalContentWriter<T>(specification, property, parameter);
				var writer = new RuntimeContentWriter<T>(_specification, condition, member);
				var result = new RuntimePipelinePart<T>(writer, parameter);
				return result;
			}
			return parameter;
		}
	}

	interface IRuntimeMemberWriters<T> : IParameterizedSource<IMember, IContentWriter<T>> { }

	sealed class RuntimeMemberWriters<T> : LocatedMemberWriters<T, MemberWriters<object, object>>, IRuntimeMemberWriters<T>
	{
		public RuntimeMemberWriters(IServiceProvider provider) : base(provider) { }
	}

	sealed class MemberWriters<T, TMember> : DecoratedMemberWriters<T>
	{
		public MemberWriters(IMemberAccessors<T, TMember> accessors, PropertyMemberWriterContent<TMember> content)
			: base(new ContentModel.Members.MemberWriters<T, TMember>(accessors, content)) { }
	}

}