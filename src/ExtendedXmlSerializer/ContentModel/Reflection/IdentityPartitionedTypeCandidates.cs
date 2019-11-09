using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class IdentityPartitionedTypeCandidates : ITypeCandidates
	{
		readonly Partition _partition;

		public IdentityPartitionedTypeCandidates(ISpecification<TypeInfo> specification, ITypeFormatter formatter)
			: this(WellKnownIdentities.Default
			                          .ToDictionary(x => x.Value.Identifier,
			                                        new TypeNamePartition(specification, formatter).Get)
			                          .Get) {}

		public IdentityPartitionedTypeCandidates(Partition partition)
		{
			_partition = partition;
		}

		public ImmutableArray<TypeInfo> Get(IIdentity parameter)
			=> _partition.Invoke(parameter.Identifier)
			             ?.Invoke(parameter.Name) ?? ImmutableArray<TypeInfo>.Empty;

		sealed class TypeNamePartition
			: IParameterizedSource<KeyValuePair<Assembly, IIdentity>, Func<string, ImmutableArray<TypeInfo>?>>
		{
			readonly static ApplicationTypes ApplicationTypes = ApplicationTypes.Default;

			readonly IApplicationTypes      _types;
			readonly Func<TypeInfo, bool>   _specification;
			readonly Func<TypeInfo, string> _formatter;

			public TypeNamePartition(ISpecification<TypeInfo> specification, ITypeFormatter formatter)
				: this(ApplicationTypes, specification.IsSatisfiedBy, formatter.Get) {}

			TypeNamePartition(IApplicationTypes types, Func<TypeInfo, bool> specification,
			                  Func<TypeInfo, string> formatter)
			{
				_types         = types;
				_specification = specification;
				_formatter     = formatter;
			}

			public Func<string, ImmutableArray<TypeInfo>?> Get(KeyValuePair<Assembly, IIdentity> parameter)
				=> _types.Get(parameter.Key)
				         .Where(_specification)
				         .ToLookup(_formatter)
				         .ToDictionary(y => y.Key, y => y.ToImmutableArray())
				         .GetStructure;
		}
	}
}