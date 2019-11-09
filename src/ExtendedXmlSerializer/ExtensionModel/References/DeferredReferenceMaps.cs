using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredReferenceMaps : AssociationAwareReferenceMaps
	{
		public DeferredReferenceMaps(IReferenceMaps maps) : base(new Implementation(maps)) {}

		sealed class Implementation : ReferenceCacheBase<IFormatReader, IReferenceMap>, IReferenceMaps
		{
			readonly IContentsHistory  _contexts;
			readonly IDeferredCommands _commands;
			readonly IReferenceMaps    _maps;

			[UsedImplicitly]
			public Implementation(IReferenceMaps maps) :
				this(ContentsHistory.Default, DeferredCommands.Default, maps) {}

			public Implementation(IContentsHistory contexts, IDeferredCommands commands, IReferenceMaps maps)
			{
				_contexts = contexts;
				_commands = commands;
				_maps     = maps;
			}

			protected override IReferenceMap Create(IFormatReader parameter)
				=> new DeferredReferenceMap(_commands.Get(parameter), _contexts.Get(parameter), _maps.Get(parameter));
		}
	}
}