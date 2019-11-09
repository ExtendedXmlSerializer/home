using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel
{
	class GenericWriterAdapter<T> : IWriter
	{
		readonly IWriter<T> _writer;

		public GenericWriterAdapter(IWriter<T> writer) => _writer = writer;

		public void Write(IFormatWriter writer, object instance) => _writer.Write(writer, (T)instance);
	}
}