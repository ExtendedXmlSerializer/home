using JetBrains.Annotations;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassPropertyType
	{
		public string NormalProp { [UsedImplicitly] get; set; }
		public string OnlyGetProp { [UsedImplicitly] get; set; }

		public static string StaticProp { [UsedImplicitly] get; set; }
		public virtual string VirtualProp { [UsedImplicitly] get; set; }

		public          string NormalField;
		public readonly string ReadonlyField = "6";
		public const    string ConstField    = "7";
		public static   string StaticField;

		public void Init()
		{
			NormalProp  = "1";
			OnlyGetProp = "2";
			StaticProp  = "3";
			VirtualProp = "4";
			NormalField = "5";
			StaticField = "8";
		}
	}
}