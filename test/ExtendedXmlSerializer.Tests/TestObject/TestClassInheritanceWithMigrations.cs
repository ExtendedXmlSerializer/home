namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassInheritanceWithMigrationsBase
	{
		public int ChangedProperty { get; set; }
	}

	public class TestClassInheritanceWithMigrationsA : TestClassInheritanceWithMigrationsBase
	{
		public int OtherChangedProperty { get; set; }
	}

	public class TestClassInheritanceWithMigrationsB : TestClassInheritanceWithMigrationsBase
	{
		public int ProprtyWithoutChanges { get; set; }
	}
}