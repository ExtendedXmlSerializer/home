using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	public class CollectionSpecification : DecoratedSpecification<TypeInfo>
	{
		public CollectionSpecification(ISpecification<TypeInfo> specification) :
			base(IsCollectionTypeSpecification.Default.And(specification)) {}
	}
}