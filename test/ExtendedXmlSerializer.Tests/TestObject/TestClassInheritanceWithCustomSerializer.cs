namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassInheritanceWithCustomSerializerBase
	{
		public string FirstProperty { get; set; }
	}

	public class TestClassInheritanceWithCustomSerializerA : TestClassInheritanceWithCustomSerializerBase
	{
		public string SecondProperty { get; set; }
	}

	public class TestClassInheritanceWithCustomSerializerB : TestClassInheritanceWithCustomSerializerBase
	{
		public string ThirdProperty { get; set; }
	}
}