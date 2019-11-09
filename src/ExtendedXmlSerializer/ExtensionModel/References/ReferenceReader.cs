using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceReader : DecoratedReader
	{
		readonly static ContentModel.Properties.ReferenceIdentity ReferenceIdentity =
			ContentModel.Properties.ReferenceIdentity.Default;

		readonly IReferenceMaps  _maps;
		readonly IEntities       _entities;
		readonly TypeInfo        _definition;
		readonly IClassification _classification;

		// ReSharper disable once TooManyDependencies
		public ReferenceReader(IReader reader, IReferenceMaps maps, IEntities entities, TypeInfo definition,
		                       IClassification classification) : base(reader)
		{
			_maps           = maps;
			_entities       = entities;
			_definition     = definition;
			_classification = classification;
		}

		ReferenceIdentity? GetReference(IFormatReader parameter)
		{
			var identity = ReferenceIdentity.Get(parameter);
			if (identity.HasValue)
			{
				return new ReferenceIdentity(identity.Value);
			}

			var type = _classification.GetClassification(parameter, _definition);
			var entity = _entities.Get(type)
			                      ?.Reference(parameter);
			if (entity != null)
			{
				return new ReferenceIdentity(type, entity);
			}

			return null;
		}

		public override object Get(IFormatReader parameter)
		{
			var identity = GetReference(parameter);
			if (identity != null)
			{
				var map       = _maps.Get(parameter);
				var reference = map.Get(identity.Value);
				return reference;
			}

			var result = base.Get(parameter);
			return result;
		}
	}
}