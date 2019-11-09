namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassWithSerializer
	{
		public TestClassWithSerializer(string paramStr, int paramInt)
		{
			PropStr = paramStr;
			PropInt = paramInt;
		}

		public string PropStr { get; private set; }
		public int PropInt { get; private set; }
	}
}