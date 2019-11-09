using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class ReaderAdapter<T> : IReader<T>
	{
		readonly IReader _serializer;

		public ReaderAdapter(IReader serializer)
		{
			_serializer = serializer;
		}

		public T Get(IFormatReader parameter) => (T)_serializer.Get(parameter);
	}
}