using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Properties;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceSerializer : ISerializer
	{
		readonly IReferenceEncounters _encounters;
		readonly IReader              _reader;
		readonly IWriter              _writer;

		public ReferenceSerializer(IReferenceEncounters encounters, IReader reader, IWriter writer)
		{
			_encounters = encounters;
			_reader     = reader;
			_writer     = writer;
		}

		public void Write(IFormatWriter writer, object instance)
		{
			var encounters = _encounters.Get(writer);
			var identifier = encounters.Get(instance);
			if (identifier != null)
			{
				var first  = encounters.IsSatisfiedBy(instance);
				var entity = identifier.Value.Entity;
				if (entity != null)
				{
					if (!first)
					{
						EntityIdentity.Default.Write(writer, entity.Get(instance));
					}
				}
				else
				{
					var property = first
						               ? (IProperty<uint?>)IdentityProperty.Default
						               : ContentModel.Properties.ReferenceIdentity.Default;
					property.Write(writer, identifier.Value.UniqueId);
				}

				if (!first)
				{
					return;
				}
			}

			_writer.Write(writer, instance);
		}

		public object Get(IFormatReader parameter) => _reader.Get(parameter);
	}
}