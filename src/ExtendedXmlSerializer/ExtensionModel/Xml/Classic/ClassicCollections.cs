using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class ClassicCollections : Collections
	{
		public ClassicCollections(RuntimeSerializers serializers, ClassicCollectionContents contents)
			: base(serializers, contents) {}
	}
}