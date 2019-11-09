using System;
using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	sealed class ParsedContent<T> : IParameterizedSource<IFormatReader, T>
	{
		readonly Func<IFormatReader, IParser<T>> _source;

		public ParsedContent(Func<IFormatReader, IParser<T>> source)
		{
			_source = source;
		}

		public T Get(IFormatReader parameter) => _source(parameter)
			.Get(parameter.Content());
	}
}