using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class DefaultCollectionSpecification : CollectionSpecification
	{
		public DefaultCollectionSpecification(IActivatingTypeSpecification specification) : base(specification) {}
	}
}