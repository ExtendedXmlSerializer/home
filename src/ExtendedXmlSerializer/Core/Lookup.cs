using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core
{
	public static class Lookup
	{
		public static KeyValuePair<T1, T2> Create<T1, T2>(T1 item1, T2 item2) => new KeyValuePair<T1, T2>(item1, item2);
	}
}