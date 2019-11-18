namespace ExtendedXmlSerializer.Tests.ReportedIssues.Support
{
	public interface ISerializationSupport : IExtendedXmlSerializer
	{
		T Assert<T>(T instance, string expected);
	}
}