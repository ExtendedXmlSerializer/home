using System.Collections.Specialized;
using System.Diagnostics;

namespace ExtendedXmlSerializer.Core.Collections {
	/// <summary>
	/// ATTRIBUTION: https://github.com/mattmc3/dotmore
	/// </summary>
	class OrderedDictionaryDebugView
	{

		private IOrderedDictionary _dict;
		public OrderedDictionaryDebugView(IOrderedDictionary dict) => _dict = dict;

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public IndexedKeyValuePairs[] IndexedKeyValuePairs
		{
			get
			{
				IndexedKeyValuePairs[] nkeys = new IndexedKeyValuePairs[_dict.Count];

				int i = 0;
				foreach (object key in _dict.Keys)
				{
					nkeys[i] = new IndexedKeyValuePairs(_dict, i, key, _dict[key]);
					i += 1;
				}
				return nkeys;
			}
		}
	}
}