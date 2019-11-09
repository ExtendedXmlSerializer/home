using JetBrains.Annotations;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassGeneric<T>
	{
		public T GenericProp { [UsedImplicitly] get; set; }
		public string Normal { [UsedImplicitly] get; set; }

		public void Init(T genericValue)
		{
			Normal      = "normal";
			GenericProp = genericValue;
		}
	}

	public class TestClassGenericThree<T, TK, TL>
	{
		public T GenericProp { get; set; }
		public TK GenericProp2 { get; set; }
		public TL GenericProp3 { get; set; }
		public string Normal { [UsedImplicitly] get; set; }

		public void Init(T genericValue, TK genericValue2, TL genericValue3)
		{
			Normal       = "normal";
			GenericProp  = genericValue;
			GenericProp2 = genericValue2;
			GenericProp3 = genericValue3;
		}
	}

	public class TestClassPropGeneric
	{
		public TestClassGenericThree<string, int, decimal> PropGenric { get; set; }
	}
}