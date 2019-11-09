namespace ExtendedXmlSerializer.ContentModel.Members
{
	public interface IMemberSerializer : ISerializer
	{
		IMember Profile { get; }

		IMemberAccess Access { get; }
	}
}