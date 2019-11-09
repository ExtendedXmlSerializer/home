using System;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Parsing;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	sealed class DelegatedParsingReader<T> : DecoratedReader<T>
	{
		public DelegatedParsingReader(Func<string, T> format, IIdentity identity)
			: base(new ParsingReader<T>(new DelegatedParser<T>(format), identity)) {}
	}
}