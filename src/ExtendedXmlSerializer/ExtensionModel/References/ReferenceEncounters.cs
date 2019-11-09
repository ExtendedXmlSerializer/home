using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceEncounters : ReferenceCacheBase<IFormatWriter, IEncounters>, IReferenceEncounters
	{
		readonly IRootReferences   _references;
		readonly IEntities         _entities;
		readonly ObjectIdGenerator _generator;

		[UsedImplicitly]
		public ReferenceEncounters(IRootReferences references, IEntities entities)
			: this(references, entities, new ObjectIdGenerator()) {}

		public ReferenceEncounters(IRootReferences references, IEntities entities, ObjectIdGenerator generator)
		{
			_references = references;
			_entities   = entities;
			_generator  = generator;
		}

		protected override IEncounters Create(IFormatWriter parameter)
			=> new Encounters(_references.Get(parameter)
			                             .ToDictionary(x => x, Get));

		Identifier Get(object parameter)
			=> new Identifier(_generator.For(parameter), _entities.Get(parameter.GetType()
			                                                                    .GetTypeInfo()));
	}
}