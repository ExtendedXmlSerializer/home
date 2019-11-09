using System;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	sealed class DelegatedFormattingWriter<T> : DecoratedWriter<T>
	{
		public DelegatedFormattingWriter(Func<T, string> format, IIdentity identity)
			: base(new FormattedWriter<T>(new DelegatedFormatter<T>(format), identity)) {}
	}
}