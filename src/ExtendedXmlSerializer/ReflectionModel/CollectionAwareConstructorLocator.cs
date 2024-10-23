using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class CollectionAwareConstructorLocator : ListConstructorLocator, IConstructorLocator
	{
		readonly static ISpecification<TypeInfo> Specification
			= IsInterface.Default.Or(IsGeneratedList.Default).And(IsCollectionTypeExpandedSpecification.Default);

		public CollectionAwareConstructorLocator(IConstructorLocator previous) : base(Specification, previous) {}
	}
}