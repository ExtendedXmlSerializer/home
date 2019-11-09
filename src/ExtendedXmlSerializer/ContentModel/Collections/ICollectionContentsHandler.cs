namespace ExtendedXmlSerializer.ContentModel.Collections
{
	interface ICollectionContentsHandler
	{
		void Handle(IListInnerContent contents, IReader reader);
	}
}