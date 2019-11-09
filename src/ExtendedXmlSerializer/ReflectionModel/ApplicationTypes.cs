using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ApplicationTypes : CacheBase<Assembly, ImmutableArray<TypeInfo>>, IApplicationTypes
	{
		readonly static Func<TypeInfo, bool> Specification = ApplicationTypeSpecification.Default.IsSatisfiedBy;

		public static ApplicationTypes Default { get; } = new ApplicationTypes();

		ApplicationTypes() : this(AssemblyPublicTypes.Default) {}

		public static ApplicationTypes All { get; } = new ApplicationTypes(AssemblyTypes.Default);

		readonly Func<TypeInfo, bool> _specification;
		readonly IAssemblyTypes       _types;

		public ApplicationTypes(IAssemblyTypes types) : this(Specification, types) {}

		public ApplicationTypes(Func<TypeInfo, bool> specification, IAssemblyTypes types)
		{
			_specification = specification;
			_types         = types;
		}

		protected override ImmutableArray<TypeInfo> Create(Assembly parameter) => _types.Get(parameter)
		                                                                                .Where(_specification)
		                                                                                .ToImmutableArray();
	}
}