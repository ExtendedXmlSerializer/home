using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Parsing;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	sealed class ParsingReader<T> : DecoratedReader<T>
	{
		public ParsingReader(IParser<T> formatter, IIdentity identity)
			: base(new ParsedReader<T>(formatter.Accept, identity)) {}
	}
}