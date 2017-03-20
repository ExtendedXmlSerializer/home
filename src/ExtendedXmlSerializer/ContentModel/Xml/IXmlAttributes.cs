namespace ExtendedXmlSerializer.ContentModel.Xml
{
	public interface IXmlAttributes
	{
		bool Contains(IIdentity identity);

		bool Next();
	}
}