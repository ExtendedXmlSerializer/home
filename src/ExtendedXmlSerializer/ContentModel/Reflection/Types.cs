using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class Types : CacheBase<IIdentity, TypeInfo>, ITypes
	{
		readonly ITypeIdentities _aliased;
		readonly ITypeCandidates _candidates;

		// ReSharper disable once TooManyDependencies
		[UsedImplicitly]
		public Types(IPartitionedTypeSpecification specification, IAssemblyTypePartitions partitions,
		             ITypeIdentities identities, ITypeFormatter formatter)
			: this(identities, new TypeCandidates(specification, formatter, TypeLoader.Default, partitions)) {}

		Types(ITypeIdentities aliased, ITypeCandidates candidates) : base(IdentityComparer.Default)
		{
			_aliased    = aliased;
			_candidates = candidates;
		}

		protected override TypeInfo Create(IIdentity parameter)
			=> _aliased.Get(parameter) ?? _candidates.Get(parameter).FirstOrDefault();
	}
}