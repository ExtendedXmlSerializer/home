using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	interface IFormatReaders<in T> : IParameterizedSource<T, IFormatReader> {}
}