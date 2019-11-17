using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class Serializer : ISerializer
	{
		readonly IRead   _read;
		readonly IWrite _write;

		public Serializer(IRead read, IWrite write)
		{
			_read  = read;
			_write = write;
		}

		public void Serialize(System.Xml.XmlWriter writer, object instance)
		{
			_write.Execute(new Writing(writer, instance));
		}

		public object Deserialize(System.Xml.XmlReader reader) => _read.Get(reader);
	}
}