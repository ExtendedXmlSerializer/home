using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	sealed class TypedFormattingWriter : DecoratedWriter<TypeInfo>
	{
		public TypedFormattingWriter(IIdentity identity)
			: base(new ContextualWriter<TypeInfo>(new FormattedContent<TypeInfo>(x => x).Get, identity)) {}
	}
}