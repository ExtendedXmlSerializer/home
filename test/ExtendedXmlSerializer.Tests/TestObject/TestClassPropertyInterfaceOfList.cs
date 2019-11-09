using JetBrains.Annotations;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassPropertyInterfaceOfList
	{
		public void Init()
		{
			List       = new List<string> {"Item1"};
			Set        = new HashSet<string> {"Item1"};
			Dictionary = new Dictionary<string, string> {{"Key", "Value"}};
		}

		public IList<string> List { [UsedImplicitly] get; set; }
		public IDictionary<string, string> Dictionary { [UsedImplicitly] get; set; }

		public ISet<string> Set { [UsedImplicitly] get; set; }
	}
}