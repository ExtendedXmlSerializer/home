using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Content;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class MappedArrayContents : ConditionalContents
	{
		public MappedArrayContents(IContents source)
			: base(ArraySpecification.Default.And(MappedArraySpecification.Default),
			       new MappedArraySerializers(source), source) {}
	}
}