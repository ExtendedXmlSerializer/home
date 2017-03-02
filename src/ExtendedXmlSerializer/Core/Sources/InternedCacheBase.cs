using System.Runtime.CompilerServices;

namespace ExtendedXmlSerialization.Core.Sources
{
	public abstract class InternedCacheBase<T> : ReferenceCache<string, T> where T : class
	{
		readonly IAlteration<string> _intern;

		protected InternedCacheBase(ConditionalWeakTable<string, T>.CreateValueCallback create) : this(Interns.Default, create) {}

		protected InternedCacheBase(IAlteration<string> intern, ConditionalWeakTable<string, T>.CreateValueCallback create) : base(create)
		{
			_intern = intern;
		}

		public sealed override T Get(string key) => base.Get(_intern.Get(key));
	}
}