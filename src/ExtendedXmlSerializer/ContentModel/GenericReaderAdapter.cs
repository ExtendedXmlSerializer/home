using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class GenericReaderAdapter<T> : IReader
	{
		readonly IReader<T> _serializer;

		public GenericReaderAdapter(IReader<T> serializer) => _serializer = serializer;

		object IParameterizedSource<IFormatReader, object>.Get(IFormatReader parameter) => _serializer.Get(parameter);
	}
}