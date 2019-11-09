using System.Collections.Immutable;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.Core.Parsing
{
	class ItemsParser<T> : Parsing<ImmutableArray<T>>
	{
		public ItemsParser(Parser<T> source) : this(source, Defaults.ItemDelimiter) {}

		public ItemsParser(Parser<T> source, Parser<char> delimiter)
			: base(source.DelimitedBy(delimiter)
			             .Select(x => x.ToImmutableArray())) {}
	}
}