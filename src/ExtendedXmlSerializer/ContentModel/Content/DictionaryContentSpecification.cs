using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class DictionaryContentSpecification : DecoratedSpecification<TypeInfo>
	{
		public DictionaryContentSpecification(IActivatingTypeSpecification specification) :
			base(specification.And(IsDictionaryTypeSpecification.Default)) {}
	}
}