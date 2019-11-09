using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel
{
	class DecoratedWriter : IWriter
	{
		readonly IWriter _writer;

		public DecoratedWriter(IWriter writer) => _writer = writer;

		public void Write(IFormatWriter writer, object instance) => _writer.Write(writer, instance);
	}

	class DecoratedWriter<T> : IWriter<T>
	{
		readonly IWriter<T> _writer;

		public DecoratedWriter(IWriter<T> writer) => _writer = writer;

		public void Write(IFormatWriter writer, T instance) => _writer.Write(writer, instance);
	}
}