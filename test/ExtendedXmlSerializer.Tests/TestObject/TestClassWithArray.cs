using JetBrains.Annotations;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassWithArray
	{
		public int[] ArrayOfInt { [UsedImplicitly] get; set; }
	}
}