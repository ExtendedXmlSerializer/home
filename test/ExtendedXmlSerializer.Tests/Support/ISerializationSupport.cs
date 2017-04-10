namespace ExtendedXmlSerializer.Tests.Support
{
	public interface ISerializationSupport : IExtendedXmlSerializer
	{
		T Assert<T>(T instance, string expected);
	}
}