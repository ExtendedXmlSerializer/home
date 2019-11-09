using System;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Parsing;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	class ParsedReader<T> : DecoratedReader<T>
	{
		public ParsedReader(Func<IFormatReader, IParser<T>> context, IIdentity identity)
			: base(new ContextualReader<T>(new ParsedContent<T>(context).Get, identity)) {}
	}
}