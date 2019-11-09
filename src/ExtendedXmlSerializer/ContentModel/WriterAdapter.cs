using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class WriterAdapter<T> : IWriter<T>
	{
		readonly IWriter _writer;

		public WriterAdapter(IWriter writer) => _writer = writer;

		public void Write(IFormatWriter writer, T instance) => _writer.Write(writer, instance);
	}
}