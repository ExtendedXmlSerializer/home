using System;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	sealed class FormattedContent<T> : IFormattedContent<T>
	{
		readonly Func<IFormatWriter, IFormatter<T>> _source;

		public FormattedContent(Func<IFormatWriter, IFormatter<T>> source)
		{
			_source = source;
		}

		public string Get(IFormatWriter writer, T instance) => _source(writer)
			.Get(instance);
	}
}