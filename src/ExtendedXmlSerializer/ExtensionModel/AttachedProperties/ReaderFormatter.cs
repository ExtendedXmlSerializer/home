using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	sealed class ReaderFormatter : IReaderFormatter
	{
		readonly static Delimiter Delimiter = DefaultClrDelimiters.Default.Separator;

		readonly IReaderFormatter _formatter;
		readonly Delimiter        _separator;

		public ReaderFormatter(IReaderFormatter formatter) : this(formatter, Delimiter) {}

		public ReaderFormatter(IReaderFormatter formatter, Delimiter separator)
		{
			_formatter = formatter;
			_separator = separator;
		}

		public string Get(IFormatReader parameter)
			=>
				parameter.Name.Contains((string)_separator)
					? IdentityFormatter.Default.Get(parameter.Identifier == string.Empty
						                                ? parameter.Get(parameter.Name, string.Empty)
						                                : parameter)
					: _formatter.Get(parameter);
	}
}