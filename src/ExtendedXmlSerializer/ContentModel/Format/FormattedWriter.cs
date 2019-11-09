using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	sealed class FormattedWriter<T> : DecoratedWriter<T>
	{
		public FormattedWriter(IFormatter<T> formatter, IIdentity identity)
			: base(new ContextualWriter<T>(new FormattedContent<T>(formatter.Accept).Get, identity)) {}
	}
}