using System.Runtime.CompilerServices;

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <inheritdoc />
	public class ReferenceCache<TKey, TValue> : ReferenceCacheBase<TKey, TValue>
		where TKey : class where TValue : class
	{
		readonly ConditionalWeakTable<TKey, TValue>.CreateValueCallback _callback;

		/// <inheritdoc />
		public ReferenceCache(ConditionalWeakTable<TKey, TValue>.CreateValueCallback callback) => _callback = callback;

		/// <inheritdoc />
		protected sealed override TValue Create(TKey parameter) => _callback(parameter);
	}
}