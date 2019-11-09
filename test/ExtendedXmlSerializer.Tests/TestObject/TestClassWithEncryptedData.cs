using JetBrains.Annotations;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassWithEncryptedData
	{
		public string Name { get; set; }
		public string Password { get; [UsedImplicitly] set; }
		public decimal Salary { get; set; }
	}
}