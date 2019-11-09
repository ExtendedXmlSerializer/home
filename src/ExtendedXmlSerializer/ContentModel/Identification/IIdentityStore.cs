namespace ExtendedXmlSerializer.ContentModel.Identification
{
	public interface IIdentityStore
	{
		IIdentity Get(string name, string identifier);
	}
}