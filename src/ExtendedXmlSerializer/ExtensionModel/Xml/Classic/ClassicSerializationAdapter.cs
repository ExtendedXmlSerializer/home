using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class ClassicSerializationAdapter<T> : ISerializer<T>
	{
		public static ClassicSerializationAdapter<T> Default { get; } = new ClassicSerializationAdapter<T>();

		ClassicSerializationAdapter() : this(new System.Xml.Serialization.XmlSerializer(typeof(T))) {}

		readonly System.Xml.Serialization.XmlSerializer _classic;

		public ClassicSerializationAdapter(System.Xml.Serialization.XmlSerializer classic) => _classic = classic;

		public T Get(IFormatReader parameter)
		{
			var reader = parameter.Get().AsValid<System.Xml.XmlReader>();
			reader.Read();
			var result = _classic.Deserialize(reader).AsValid<T>();
			return result;
		}

		public void Write(IFormatWriter writer, T instance)
		{
			_classic.Serialize(writer.Get().AsValid<System.Xml.XmlWriter>(), instance);
		}
	}
}