using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class MemberedCollectionWriter : IWriter
	{
		readonly IWriter _members;
		readonly IWriter _items;

		public MemberedCollectionWriter(IWriter members, IWriter items)
		{
			_members = members;
			_items   = items;
		}

		public void Write(IFormatWriter writer, object instance)
		{
			_members.Write(writer, instance);
			_items.Write(writer, instance);
		}
	}
}