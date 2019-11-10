using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypeCandidates : ITypeCandidates
	{
		readonly ITypeCandidates _identities, _reflection;

		public TypeCandidates(ISpecification<TypeInfo> specification, ITypeFormatter formatter,
		                      params ITypePartitions[] partitions)
			: this(new IdentityPartitionedTypeCandidates(specification, formatter),
			       new PartitionedTypeCandidates(partitions)) {}

		public TypeCandidates(ITypeCandidates identities, ITypeCandidates reflection)
		{
			_identities = identities;
			_reflection = reflection;
		}

		public ImmutableArray<TypeInfo> Get(IIdentity parameter)
		{
			var immutableArray = _identities.GetAny(parameter);
			var typeInfos      = _reflection.Get(parameter);
			return immutableArray ?? typeInfos;
		}
	}
}