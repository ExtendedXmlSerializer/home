namespace ExtendedXmlSerializer.Core.Sources
{
	public interface IAssignable<in TKey, in TValue>
	{
		void Assign(TKey key, TValue value);
	}
}