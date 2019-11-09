using JetBrains.Annotations;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassMapFromPrimitive
	{
		public string Wezel1 { [UsedImplicitly] get; set; }
		public int Wartosc { [UsedImplicitly] get; set; }
	}

	public class TestClassWithMap
	{
		public void Init()
		{
			ZmianaWartosci = "Stara";
			NowyWezel      = "test";
			PropClass      = new TestClassMapFromPrimitive {Wartosc = 12, Wezel1 = "WartoscWezlas"};
		}

		public string ZmianaWartosci { [UsedImplicitly] get; set; }
		public string NowyWezel { [UsedImplicitly] get; set; }
		public TestClassMapFromPrimitive PropClass { [UsedImplicitly] get; set; }
	}
}