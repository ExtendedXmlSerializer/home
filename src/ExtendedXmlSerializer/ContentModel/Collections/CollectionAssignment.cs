namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class CollectionAssignment : ICollectionAssignment
	{
		public static CollectionAssignment Default { get; } = new CollectionAssignment();

		CollectionAssignment() {}

		public void Assign(IListInnerContent contents, object item) => contents.List.Add(item);
	}
}