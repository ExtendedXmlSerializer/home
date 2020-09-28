using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;
using System.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceEncounters : ReferenceCacheBase<IFormatWriter, IEncounters>, IReferenceEncounters
	{
		readonly IRootReferences _references;
		readonly IEntities       _entities;

		[UsedImplicitly]
		public ReferenceEncounters(IRootReferences references, IEntities entities)
		{
			_references = references;
			_entities   = entities;
		}

		protected override IEncounters Create(IFormatWriter parameter)
			=> new Encounters(_references.Get(parameter).ToDictionary(x => x, new Generators(_entities).Get));

		sealed class Generators : StructureCacheBase<object, Identifier>
		{
			readonly IEntities         _entities;
			readonly ObjectIdGenerator _generator;

			public Generators(IEntities entities) : this(entities, new ObjectIdGenerator()) {}

			public Generators(IEntities entities, ObjectIdGenerator generator)
			{
				_entities  = entities;
				_generator = generator;
			}

			protected override Identifier Create(object parameter)
				=> new Identifier(_generator.For(parameter), _entities.Get(parameter.GetType()));
		}
	}
}