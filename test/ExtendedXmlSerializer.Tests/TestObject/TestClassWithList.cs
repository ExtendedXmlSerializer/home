using JetBrains.Annotations;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassFromList
	{
		public string PropString { [UsedImplicitly] get; set; }
		public int PropInt { [UsedImplicitly] get; set; }
	}

	public class TestClassWithList
	{
		public void Init()
		{
			ListStr = new List<string> {"str1", "str2"};
			ListObj = new List<TestClassFromList>
			{
				new TestClassFromList {PropString = "s1", PropInt = 1},
				new TestClassFromList {PropString = "s2", PropInt = 2}
			};
		}

		public List<string> ListStr { [UsedImplicitly] get; set; }
		public List<TestClassFromList> ListObj { [UsedImplicitly] get; set; }
	}
}