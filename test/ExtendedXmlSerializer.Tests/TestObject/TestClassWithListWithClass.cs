using JetBrains.Annotations;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassFromListWithClass
	{
		public string PropString { get; set; }
		public int PropInt { get; set; }
	}

	public class TestClassWithListWithClass
	{
		public void Init()
		{
			ListStr = new List<string> {"str1", "str2"};
			ListObj = new List<TestClassFromList>
			{
				new TestClassFromList {PropString = "s1", PropInt = 1},
				new TestClassFromList {PropString = "s2", PropInt = 2}
			};
			var obj = new TestClassPropIsInterface();
			obj.Init();
			ListWithClass = new List<TestClassPropIsInterface> {obj};
		}

		public List<string> ListStr { [UsedImplicitly] get; set; }
		public List<TestClassFromList> ListObj { [UsedImplicitly] get; set; }
		public List<TestClassPropIsInterface> ListWithClass { [UsedImplicitly] get; set; }
	}
}