using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.ContentModel.Reflection;
using System.Reflection;
using System.Xml;

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
			if (parameter.Get().To<XmlReader>().NodeType != XmlNodeType.Attribute ||
			    MemberProperty.Default.Get(parameter))
			{
				var identity = ReferenceIdentity.Get(parameter);
				if (identity.HasValue)
				{
					return new ReferenceIdentity(identity.Value);
				}

				var type   = _classification.GetClassification(parameter, _definition);
				var entity = _entities.Get(type)?.Reference(parameter);
				if (entity != null)
				{
					return new ReferenceIdentity(type, entity);
				}
			}

			return null;
		}

		public override object Get(IFormatReader parameter)
		{
			var reference = GetReference(parameter);
			if (reference != null)
			{
				var result = _maps.Get(parameter).Get(reference.Value);
				return result;
			}

			{
				var element = parameter.Get().To<XmlReader>().NodeType != XmlNodeType.Attribute ||
				              MemberProperty.Default.Get(parameter);
				var declared = element ? Identity(parameter) : null;
				var result   = base.Get(parameter);
				var identity = declared ?? (element && result != null ? Entity(parameter, result) : null);
				if (identity != null)
				{
					var map = _maps.Get(parameter);
					if (map.Get(identity.Value) != result)
					{
						map.Assign(identity.Value, result);
					}
				}

				return result;
			}
		}

		static ReferenceIdentity? Identity(IFormatReader reader)
		{
			var identity = IdentityProperty.Default.Get(reader);
			var result   = identity.HasValue ? new ReferenceIdentity(identity.Value) : (ReferenceIdentity?)null;
			return result;
		}

		ReferenceIdentity? Entity(IFormatReader reader, object instance)
		{
			var typeInfo = instance.GetType()
			                       .GetTypeInfo();
			var entity = _entities.Get(typeInfo)
			                      ?.Get(reader);
			var result = entity != null
				             ? (ReferenceIdentity?)new ReferenceIdentity(typeInfo, entity)
				             : null;
			return result;
		}
	}
}