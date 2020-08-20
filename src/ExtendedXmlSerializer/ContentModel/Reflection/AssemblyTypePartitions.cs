using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class AssemblyTypePartitions : CacheBase<Assembly, Partition>, IAssemblyTypePartitions
	{
		readonly static IApplicationTypes ApplicationTypes = ReflectionModel.ApplicationTypes.All;

		readonly IApplicationTypes                                                          _types;
		readonly Func<TypeInfo, bool>                                                       _specification;
		readonly Func<TypeInfo, string>                                                     _formatter;
		readonly Func<TypeInfo, string>                                                     _key;
		readonly Func<IGrouping<string, TypeInfo>, Func<string, ImmutableArray<TypeInfo>?>> _format;

		public AssemblyTypePartitions(IPartitionedTypeSpecification specification, ITypeFormatter formatter)
			: this(specification, formatter.Get) {}

		public AssemblyTypePartitions(ISpecification<TypeInfo> specification, Func<TypeInfo, string> formatter)
			: this(specification.IsSatisfiedBy, formatter, ApplicationTypes, x => x.Namespace) {}

		// ReSharper disable once TooManyDependencies
		public AssemblyTypePartitions(Func<TypeInfo, bool> specification, Func<TypeInfo, string> formatter,
		                              IApplicationTypes types, Func<TypeInfo, string> key)
		{
			_types         = types;
			_specification = specification;
			_formatter     = formatter;
			_key           = key;
			_format        = Format;
		}

		protected override Partition Create(Assembly parameter)
			=> _types.Get(parameter)
			         .Where(_specification)
			         .ToLookup(_key)
			         .ToDictionary(x => x.Key ?? string.Empty, _format)
			         .Get;

		public ImmutableArray<TypeInfo>? Get(TypePartition parameter)
			=> Get(parameter.Assembly)(parameter.Namespace ?? string.Empty)?.Invoke(parameter.Name);

		Func<string, ImmutableArray<TypeInfo>?> Format(IGrouping<string, TypeInfo> grouping)
			=> grouping.ToLookup(_formatter)
			           .ToDictionary(x => x.Key, x => x.ToImmutableArray())
			           .GetStructure;
	}
}