using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class TrackingReader<T> : IReader<T>
	{
		readonly IReader<T> _reader;

		public TrackingReader(IReader<T> reader) => _reader = reader;

		public T Get(IFormatReader parameter)
		{
			var result = _reader.Get(parameter);
			parameter.Set();
			return result;
		}
	}
}