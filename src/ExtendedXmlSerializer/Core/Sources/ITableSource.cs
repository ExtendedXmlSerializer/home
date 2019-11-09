using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.Core.Sources
{
	public interface ITableSource<in TKey, TValue>
		: ISpecification<TKey>,
		  IParameterizedSource<TKey, TValue>,
		  IAssignable<TKey, TValue>
	{
		bool Remove(TKey key);
	}
}