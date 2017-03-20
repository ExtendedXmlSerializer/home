using System.Collections.Generic;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public sealed class TestClassOtherClass
	{
		public static TestClassOtherClass Create()
		{
			var result = new TestClassOtherClass
			             {
				             Primitive1 = TestClassPrimitiveTypes.Create(),
				             Primitive2 = TestClassPrimitiveTypes.Create(),
				             ListProperty = new List<TestClassItem>(),
				             Other = new TestClassOther
				                     {
					                    Test = new TestClassItem {Id = 2, Name = "Other Name"},
										Double = 7.3453145324
				                     }
			             };
			for (var i = 0; i < 20; i++)
			{
				result.ListProperty.Add(new TestClassItem {Id = i, Name = $"Name 00{i.ToString()}"});
			}

			return result;
		}

		public TestClassOther Other { get; set; }
		public TestClassPrimitiveTypes Primitive1 { get; set; }
		public TestClassPrimitiveTypes Primitive2 { get; set; }

		public List<TestClassItem> ListProperty { get; set; }
	}

	public sealed class TestClassItem
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public sealed class TestClassOther
	{
		public TestClassItem Test { get; set; }

		public double Double { get; set; }
	}
}