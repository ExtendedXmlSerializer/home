using JetBrains.Annotations;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassWithObjectProperty
	{
		public object TestProperty { [UsedImplicitly] get; set; }
	}
}