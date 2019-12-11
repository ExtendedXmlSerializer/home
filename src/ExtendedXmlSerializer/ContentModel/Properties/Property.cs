using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	class Property<T> : Identity, IProperty<T>
	{
		readonly ISerializer<T> _serializer;

		public Property(IReader<T> reader, IWriter<T> writer, IIdentity identity)
			: this(new Serializer<T>(identity is FrameworkIdentity ? new TrackingReader<T>(reader) : reader, writer),
			       identity) {}

		public Property(ISerializer<T> serializer, IIdentity identity) : base(identity.Name, identity.Identifier)
			=> _serializer = serializer;

		public T Get(IFormatReader parameter) => _serializer.Get(parameter);

		public void Write(IFormatWriter writer, T instance) => _serializer.Write(writer, instance);
	}
}