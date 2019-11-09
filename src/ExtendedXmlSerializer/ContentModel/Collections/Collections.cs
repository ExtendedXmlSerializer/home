using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	public class Collections : IContents
	{
		readonly ISerializers               _serializers;
		readonly ICollectionItemTypeLocator _locator;
		readonly ICollectionContents        _contents;

		public Collections(RuntimeSerializers serializers, ICollectionContents contents)
			: this(serializers, CollectionItemTypeLocator.Default, contents) {}

		public Collections(RuntimeSerializers serializers, ICollectionItemTypeLocator locator,
		                   ICollectionContents contents)
		{
			_serializers = serializers;
			_locator     = locator;
			_contents    = contents;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var itemType   = _locator.Get(parameter);
			var serializer = _serializers.Get(itemType);
			var item       = new Serializer(serializer, new CollectionItemAwareWriter(serializer));
			var result     = _contents.Get(new CollectionContentInput(item, parameter, itemType));
			return result;
		}
	}
}