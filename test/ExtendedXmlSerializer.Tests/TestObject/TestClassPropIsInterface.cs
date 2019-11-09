using JetBrains.Annotations;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public interface ITestInterface
	{
		string PropFromInterface { get; set; }
	}

	public class TestClassInheritInterface1 : ITestInterface
	{
		public string PropFromInterface { get; set; }
	}

	public class TestClassInheritInterface2 : ITestInterface
	{
		public string PropFromInterface { get; set; }
		public string PropFromClass { [UsedImplicitly] get; set; }
	}

	public class TestClassPropIsInterface
	{
		public void Init()
		{
			PropInter1 = new TestClassInheritInterface1 {PropFromInterface = "PropInter1"};
			PropInter2 = new TestClassInheritInterface2
				{PropFromInterface = "PropInter2", PropFromClass = "PropClass1"};
		}

		public ITestInterface PropInter1 { [UsedImplicitly] get; set; }
		public ITestInterface PropInter2 { [UsedImplicitly] get; set; }
	}
}