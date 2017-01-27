using ExtendedXmlSerialization.ElementModel.Names;

namespace ExtendedXmlSerialization.ElementModel
{
	public interface ICollectionItem : IContainer, IName {}

	public class CollectionItem : ContainerBase, ICollectionItem
	{
		public CollectionItem(INamedElement element) : this(element.Name.DisplayName, element) {}

		public CollectionItem(string displayName, IElement element) : base(element)
		{
			DisplayName = displayName;
		}

		public string DisplayName { get; }
	}
}