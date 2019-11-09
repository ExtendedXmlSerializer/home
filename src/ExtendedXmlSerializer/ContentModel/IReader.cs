using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel
{
	public interface IReader : IReader<object> {}

	public interface IReader<out T> : IParameterizedSource<IFormatReader, T> {}
}