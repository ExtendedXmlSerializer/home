using ExtendedXmlSerializer.ContentModel.Members;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class CollectionContentsHandler : ICollectionContentsHandler
	{
		readonly ICollectionAssignment _assignment;

		public CollectionContentsHandler(ICollectionAssignment assignment) => _assignment = assignment;

		public void Handle(IListInnerContent contents, IReader reader)
		{
			_assignment.Assign(contents, reader.GetIfAssigned(contents.Get()));
		}
	}
}