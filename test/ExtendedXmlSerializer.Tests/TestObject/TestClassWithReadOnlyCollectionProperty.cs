using System.Collections.Generic;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public sealed class TestClassWithReadOnlyCollectionProperty
	{
		public IList<TestClassPrimitiveTypes> Items { get; } = new List<TestClassPrimitiveTypes>();
	}
}