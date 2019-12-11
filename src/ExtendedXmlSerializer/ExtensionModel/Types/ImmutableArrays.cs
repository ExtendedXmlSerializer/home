using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ImmutableArrays : Collections
	{
		public ImmutableArrays(RuntimeSerializers serializers, ImmutableArrayContents contents) 
			: base(serializers, contents) {}
	}
}