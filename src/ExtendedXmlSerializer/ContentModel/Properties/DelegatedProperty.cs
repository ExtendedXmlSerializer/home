using System;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	class DelegatedProperty<T> : Property<T>
	{
		public DelegatedProperty(Func<string, T> parser, Func<T, string> formatter, IIdentity identity)
			: base(new DelegatedParsingReader<T>(parser, identity),
			       new DelegatedFormattingWriter<T>(formatter, identity), identity) {}
	}
}