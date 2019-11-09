using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using Sprache;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class PartitionedTypeCandidates : StructureCacheBase<IIdentity, ImmutableArray<TypeInfo>>, ITypeCandidates
	{
		readonly static AssemblyPathParser AssemblyPathParser = AssemblyPathParser.Default;
		readonly static AssemblyLoader     AssemblyLoader     = AssemblyLoader.Default;

		public PartitionedTypeCandidates(params ITypePartitions[] partitions)
			: this(AssemblyPathParser, AssemblyLoader,
			       new TypePartitions(partitions
				                          .ToArray<IParameterizedSource<TypePartition, ImmutableArray<TypeInfo>?>
				                          >())) {}

		readonly Parser<AssemblyPath> _parser;
		readonly IAssemblyLoader      _loader;
		readonly ITypePartitions      _partitions;

		public PartitionedTypeCandidates(Parser<AssemblyPath> parser, IAssemblyLoader loader,
		                                 ITypePartitions partitions)
		{
			_parser     = parser;
			_loader     = loader;
			_partitions = partitions;
		}

		protected override ImmutableArray<TypeInfo> Create(IIdentity parameter)
		{
			var parse = _parser.TryParse(parameter.Identifier);
			if (parse.WasSuccessful)
			{
				var path      = parse.Value;
				var partition = new TypePartition(_loader.Get(path.Path), path.Namespace, parameter.Name);
				var array     = _partitions.Get(partition);
				var result    = array ?? ImmutableArray<TypeInfo>.Empty;
				return result;
			}

			return ImmutableArray<TypeInfo>.Empty;
		}
	}
}