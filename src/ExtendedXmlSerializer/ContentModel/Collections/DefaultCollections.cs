namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class DefaultCollections : Collections
	{
		public DefaultCollections(RuntimeSerializers serializers, CollectionContents contents)
			: base(serializers, contents) {}
	}
}