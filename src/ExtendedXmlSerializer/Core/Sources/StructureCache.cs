using System;

namespace ExtendedXmlSerializer.Core.Sources
{
	class StructureCache<TKey, TValue> : StructureCacheBase<TKey, TValue> where TKey : class where TValue : struct
	{
		readonly Func<TKey, TValue> _factory;

		public StructureCache(Func<TKey, TValue> factory)
		{
			_factory = factory;
		}

		protected sealed override TValue Create(TKey parameter) => _factory(parameter);
	}
}