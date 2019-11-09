using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	interface IFormatWriters<in T> : IParameterizedSource<T, IFormatWriter> {}
}