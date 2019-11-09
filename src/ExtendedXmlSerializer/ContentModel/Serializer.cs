using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;

namespace ExtendedXmlSerializer.ContentModel
{
	class Serializer : Serializer<object>, ISerializer
	{
		public Serializer(IReader reader, IWriter writer) : base(reader, writer) {}
	}

	class Serializer<T> : ISerializer<T>
	{
		readonly IReader<T> _reader;
		readonly IWriter<T> _writer;

		public Serializer(IReader<T> reader, IWriter<T> writer)
		{
			_reader = reader;
			_writer = writer;
		}

		public T Get(IFormatReader parameter) => _reader.GetIfAssigned(parameter);

		public void Write(IFormatWriter writer, T instance) => _writer.Write(writer, instance);
	}
}