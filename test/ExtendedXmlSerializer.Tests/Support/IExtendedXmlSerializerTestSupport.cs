namespace ExtendedXmlSerializer.Tests.Support
{
	public interface IExtendedXmlSerializerTestSupport : IExtendedXmlSerializer
	{
		T Assert<T>(T instance, string expected);
	}
}