namespace ExtendedXmlSerializer.Configuration
{
	interface IInternalMemberConfiguration
	{
		IMemberConfiguration Name(string name);

		IMemberConfiguration Order(int order);
	}
}