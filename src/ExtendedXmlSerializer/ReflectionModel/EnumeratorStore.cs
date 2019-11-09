using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class EnumeratorStore : DecoratedSource<TypeInfo, IEnumerators>, IEnumeratorStore
	{
		public EnumeratorStore(IDictionaryEnumerators dictionary, IEnumerators enumerators)
			: base(enumerators.If(IsCollectionTypeSpecification.Default)
			                  .Let(IsDictionaryTypeSpecification.Default, dictionary)) {}
	}
}