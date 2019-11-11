using ExtendedXmlSerializer.ExtensionModel.Instances;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.IO;
using System.Text;
using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public sealed class ReferencedDeserialization<T> where T : class
	{
		readonly IInstanceReader   _reader;
		readonly T                 _existing;
		readonly XmlReaderSettings _settings;

		public ReferencedDeserialization(IExtendedXmlSerializer @this, T existing)
			: this(@this, existing, ExtensionModel.Xml.Defaults.ReaderSettings) {}

		public ReferencedDeserialization(IExtendedXmlSerializer serializer, T existing, XmlReaderSettings settings)
			: this(InstanceReaders.Default.Get(serializer), existing, settings) {}

		ReferencedDeserialization(IInstanceReader reader, T existing, XmlReaderSettings settings)
		{
			_reader   = reader;
			_existing = existing;
			_settings = settings;
		}

		public T Deserialize(string data) => Deserialize(Xml.Defaults.CloseRead, data);

		public T Deserialize(XmlReaderSettings settings, string data)
			=> Deserialize(settings, new MemoryStream(Encoding.UTF8.GetBytes(data)));

		public T Deserialize(Stream stream) => Deserialize(_settings, stream);

		public T Deserialize(XmlReaderSettings settings, Stream stream)
		{
			var reader   = new XmlReaderFactory(settings, settings.NameTable.Context()).Get(stream);
			var existing = new Existing(reader, _existing);
			var result   = (T)_reader.Get(existing);
			return result;
		}
	}
}