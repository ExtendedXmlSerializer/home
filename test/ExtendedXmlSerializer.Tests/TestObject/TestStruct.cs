using JetBrains.Annotations;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public struct TestStruct
	{
		public void Init()
		{
			A = 1;
			B = 2;
		}

		public int A;
		public int B { [UsedImplicitly] get; set; }

		public const int C = 3;
	}
}