using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel
{
	class DecoratedReader : IReader
	{
		readonly IReader _reader;

		public DecoratedReader(IReader reader)
		{
			_reader = reader;
		}

		public virtual object Get(IFormatReader parameter) => _reader.Get(parameter);
	}

	class DecoratedReader<T> : IReader<T>
	{
		readonly IReader<T> _reader;

		public DecoratedReader(IReader<T> reader)
		{
			_reader = reader;
		}

		public T Get(IFormatReader parameter) => _reader.Get(parameter);
	}
}