namespace ExtendedXmlSerializer.Core.Sources
{
	/// <summary>
	/// A command-based interface that accepts a key and value, usually to pair the two with a store of some sort.
	/// </summary>
	/// <typeparam name="TKey">The key type.</typeparam>
	/// <typeparam name="TValue">The value type.</typeparam>
	public interface IAssignable<in TKey, in TValue>
	{
		/// <summary>
		/// Assigns a value with the associated key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		void Assign(TKey key, TValue value);
	}
}