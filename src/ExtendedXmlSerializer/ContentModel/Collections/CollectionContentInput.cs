using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	readonly struct CollectionContentInput
	{
		public CollectionContentInput(ISerializer item, TypeInfo classification, TypeInfo itemType)
		{
			Item           = item;
			Classification = classification;
			ItemType       = itemType;
		}

		public ISerializer Item { get; }

		public TypeInfo Classification { get; }

		public TypeInfo ItemType { get; }
	}
}