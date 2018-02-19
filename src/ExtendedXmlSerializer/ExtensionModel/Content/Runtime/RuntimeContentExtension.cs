using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Runtime
{
	struct Registration
	{
		public Registration(GroupName name, IRuntimePipelineComposer<object> instance)
			: this(name, new InstanceService<IRuntimePipelineComposer<object>>(instance)) {}

		public Registration(GroupName name, IService<IRuntimePipelineComposer<object>> selection)
		{
			Name = name;
			Selection = selection;
		}

		public GroupName Name { get; }

		public IService<IRuntimePipelineComposer<object>> Selection { get; }
	}

	sealed class RuntimeMembersExtension : ISerializerExtension, IAssignable<MemberInfo, Registration>, ICommand<MemberInfo>
	{
		readonly IMemberRuntimeRegistry _registry;

		[UsedImplicitly]
		public RuntimeMembersExtension() : this(new MemberRuntimeRegistry()) { }

		public RuntimeMembersExtension(IMemberRuntimeRegistry registry) => _registry = registry;

		public IServiceRepository Get(IServiceRepository parameter)
				=> parameter.RegisterDefinition<IRuntimeRegistry<object>, RuntimeRegistry<object>>()
							.RegisterInstance(_registry);

		void ICommand<IServices>.Execute(IServices parameter) { }

		public void Execute(MemberInfo parameter)
		{
			_registry.Remove(parameter);
		}

		public void Execute(KeyValuePair<MemberInfo, Registration> parameter)
		{
			_registry.Get(parameter.Key)
			         .Get(parameter.Value.Name)
			         .Add(parameter.Value.Selection);
		}
	}

	static class RuntimeExtensions
	{
		public static MemberConfiguration<T, TMember> OnlyEmitWhen<T, TMember>(this MemberConfiguration<T, TMember> @this, Func<TMember, bool> specification)
			=> OnlyEmitWhen(@this, new DelegatedSpecification<TMember>(specification));

		public static MemberConfiguration<T, TMember> OnlyEmitWhen<T, TMember>(this MemberConfiguration<T, TMember> @this, ISpecification<TMember> specification)
			=> @this.Extend<RuntimeMembersExtension>()
			        .Assign(@this, PipelinePhases.Validation,
			                new EmitRuntimePipelineComposer<TMember>(specification))
			        .Return(@this);

		public static RuntimeMembersExtension Assign<T>(this RuntimeMembersExtension @this, IConfigurationElement member,
		                                                GroupName name, IRuntimePipelineComposer<T> selection)
			=> @this.Assign(member.Member(), new Registration(name, selection.Generalized())).Return(@this);

		public static IRuntimePipelineComposer<object> Generalized<T>(this IRuntimePipelineComposer<T> @this)
			=> new GenerializedRuntimePipelineComposer<T>(@this);
	}

	interface IRuntimePipelineComposer<T> : IAlteration<IRuntimePipelinePart<T>> {}

	class DecoratedRuntimePipelineComposer<T> : DecoratedAlteration<IRuntimePipelinePart<T>>, IRuntimePipelineComposer<T>
	{
		public DecoratedRuntimePipelineComposer(IRuntimePipelineComposer<T> source) : base(source) {}
	}

	class GenerializedRuntimePipelineComposer<T> : RuntimePipelineComposerMarker, IRuntimePipelineComposer<T>
	{
		readonly IRuntimePipelineComposer<T> _selection;

		public GenerializedRuntimePipelineComposer(IRuntimePipelineComposer<T> selection) => _selection = selection;

		public IRuntimePipelinePart<T> Get(IRuntimePipelinePart<T> parameter) => _selection.Get(parameter);
	}

	class RuntimePipelineComposerMarker : IRuntimePipelineComposer<object>
	{
		public IRuntimePipelinePart<object> Get(IRuntimePipelinePart<object> parameter)
			=> throw new NotSupportedException("This is used for static type-checking purposes only.");
	}

	sealed class EmitRuntimePipelineComposer<T> : IRuntimePipelineComposer<T>
	{
		readonly ISpecification<T> _specification;

		public EmitRuntimePipelineComposer(ISpecification<T> specification) => _specification = specification;

		public IRuntimePipelinePart<T> Get(IRuntimePipelinePart<T> parameter)
		{
			var content = new SpecificationContentWriter<T>(_specification, parameter);
			var writer = new RuntimeContentWriter<T>(parameter, content, parameter.Get());
			var result = new RuntimePipelinePart<T>(writer, parameter);
			return result;
		}
	}

	interface IMemberRuntimeRegistry : IMemberTable<IRuntimeRegistryStore>, IRegistration {}

	sealed class MemberRuntimeRegistry : MetadataTable<IRuntimeRegistryStore>, IMemberRuntimeRegistry
	{
		readonly IRegistration _registration;

		public MemberRuntimeRegistry() : this(new RuntimeStoreTable<TypeInfo>(ReflectionModel.Defaults.TypeComparer),
		                                      new RuntimeStoreTable<MemberInfo>(MemberComparer.Default)) {}

		public MemberRuntimeRegistry(IRuntimeStoreTable<TypeInfo> types, IRuntimeStoreTable<MemberInfo> members)
			: this(types, members, new CompositeRegistration(types, members)) {}

		public MemberRuntimeRegistry(IRuntimeStoreTable<TypeInfo> types, IRuntimeStoreTable<MemberInfo> members,
		                             IRegistration registration)
			: base(types, members) => _registration = registration;

		public IServiceRepository Get(IServiceRepository parameter) => _registration.Get(parameter);
	}

	interface IRuntimeStoreTable<T> : IMetadataTable<T, IRuntimeRegistryStore>, IRegistration where T : MemberInfo {}

	sealed class RuntimeStoreTable<T> : Cache<T, IRuntimeRegistryStore>, IRuntimeStoreTable<T> where T : MemberInfo
	{
		readonly IRegistration _registration;

		public RuntimeStoreTable(IEqualityComparer<T> comparer) : this(new ConcurrentDictionary<T, IRuntimeRegistryStore>(comparer)) {}

		public RuntimeStoreTable(ConcurrentDictionary<T, IRuntimeRegistryStore> store)
			: this(store, new ItemRegistration<IRuntimeRegistryStore>(new Values<T, IRuntimeRegistryStore>(store))) {}

		public RuntimeStoreTable(ConcurrentDictionary<T, IRuntimeRegistryStore> store, IRegistration registration)
			: base(_ => new RuntimeRegistryStore(), store) => _registration = registration;

		public IServiceRepository Get(IServiceRepository parameter) => _registration.Get(parameter);
	}

	interface IMemberRuntimeRegistry<T> : ISpecificationSource<MemberInfo, IEnumerable<IService<IRuntimePipelineComposer<T>>>> { }

	sealed class MemberRuntimeRegistry<T> : DecoratedSpecification<MemberInfo>, IMemberRuntimeRegistry<T>
	{
		readonly IMemberRuntimeRegistry _registry;
		readonly Func<IService<IRuntimePipelineComposer<object>>, IService<IRuntimePipelineComposer<T>>> _alter;

		public MemberRuntimeRegistry(IMemberRuntimeRegistry registry)
			: this(registry, RuntimePipelineItemCoercer<T>.Default.ToDelegate()) {}

		public MemberRuntimeRegistry(IMemberRuntimeRegistry registry,
		                             Func<IService<IRuntimePipelineComposer<object>>,
			                              IService<IRuntimePipelineComposer<T>>> alter) : base(registry)
		{
			_registry = registry;
			_alter = alter;
		}

		public IEnumerable<IService<IRuntimePipelineComposer<T>>> Get(MemberInfo parameter) => _registry.Get(parameter)
		                                                                                                .Reverse()
		                                                                                                .Select(_alter);
	}

	sealed class RuntimePipelineItemCoercer<T>
		: DecoratedSource<IService<IRuntimePipelineComposer<object>>, IService<IRuntimePipelineComposer<T>>>
	{
		public static RuntimePipelineItemCoercer<T> Default { get; } = new RuntimePipelineItemCoercer<T>();

		RuntimePipelineItemCoercer() : base(CastCoercer<IService<IRuntimePipelineComposer<object>>,
			                                        IService<IRuntimePipelineComposer<T>>>.Default) {}
	}



	// ReSharper disable once PossibleInfiniteInheritance
	interface IRuntimeRegistry<T> : IParameterizedSource<MemberInfo, ImmutableArray<IRuntimePipelineComposer<T>>> {}

	// ReSharper disable once PossibleInfiniteInheritance
	sealed class RuntimeRegistry<T> : IRuntimeRegistry<T>
	{
		readonly Func<IService<IRuntimePipelineComposer<T>>, IRuntimePipelineComposer<T>> _services;
		readonly IMemberRuntimeRegistry<T> _members;

		public RuntimeRegistry(IServiceCoercer<IRuntimePipelineComposer<T>> coercer, IMemberRuntimeRegistry<T> members)
			: this(coercer.Get, members) {}

		public RuntimeRegistry(Func<IService<IRuntimePipelineComposer<T>>, IRuntimePipelineComposer<T>> services, IMemberRuntimeRegistry<T> members)
		{
			_services = services;
			_members = members;
		}

		public ImmutableArray<IRuntimePipelineComposer<T>> Get(MemberInfo parameter)
			=> _members.IsSatisfiedBy(parameter)
				   ? _members.Get(parameter)
				             .Select(_services)
				             .ToImmutableArray()
				   : ImmutableArray<IRuntimePipelineComposer<T>>.Empty;
	}

	public class PipelinePhases : GroupNames
	{
		public static GroupName Start = new GroupName("Start"),
		                        Validation = new GroupName("Validation"),
		                        Input = new GroupName("Input"),
		                        Content = new GroupName("Content"),
		                        Finish = new GroupName("Finish");


		public static PipelinePhases Default { get; } = new PipelinePhases();
		PipelinePhases() : base(Start, Validation, Input, Content, Finish) {}
	}

	sealed class RuntimePipelineGroups : ServiceGroups<IRuntimePipelineComposer<object>>
	{
		public static RuntimePipelineGroups Default { get; } = new RuntimePipelineGroups();

		RuntimePipelineGroups() : this(PipelinePhases.Default) {}

		public RuntimePipelineGroups(IEnumerable<GroupName> phases) : base(phases) {}
	}

	interface IRuntimeRegistryStore : IServiceGroupCollection<IRuntimePipelineComposer<object>>, IRegistration { }

	sealed class RuntimeRegistryStore : ServiceGroupCollection<IRuntimePipelineComposer<object>>, IRuntimeRegistryStore
	{
		public RuntimeRegistryStore() : base(RuntimePipelineGroups.Default) {}
	}

	sealed class RuntimeContentComposer<T> : IRuntimeContentComposer<T>
	{
		readonly IRuntimeRegistry<T> _registrations;

		public RuntimeContentComposer(IRuntimeRegistry<T> registry) => _registrations = registry;

		public IRuntimeContent<T> Get(IRuntimePipelinePart<T> parameter)
			=> _registrations.Get(parameter.Get()
			                               .Metadata)
			                 .Alter(parameter);
	}
}
