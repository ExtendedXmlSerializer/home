using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	class CollectionSpecification : DecoratedSpecification<TypeInfo>
	{
		public CollectionSpecification(ISpecification<TypeInfo> specification)
			: base(IsCollectionTypeSpecification.Default.And(specification)) {}
	}
}