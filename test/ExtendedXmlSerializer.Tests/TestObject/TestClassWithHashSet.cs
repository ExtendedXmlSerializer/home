using JetBrains.Annotations;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassFromHashSet
	{
		public string PropString { get; set; }
		public int PropInt { get; set; }
	}

	public class TestClassWithHashSet
	{
		public void Init()
		{
			ListStr = new HashSet<string> {"str1", "str2"};
			ListObj = new HashSet<TestClassFromList>
			{
				new TestClassFromList {PropString = "s1", PropInt = 1},
				new TestClassFromList {PropString = "s2", PropInt = 2}
			};
		}

		public HashSet<string> ListStr { [UsedImplicitly] get; set; }
		public HashSet<TestClassFromList> ListObj { [UsedImplicitly] get; set; }
	}
}