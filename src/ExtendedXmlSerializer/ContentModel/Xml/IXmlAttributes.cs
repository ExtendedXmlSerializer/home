namespace ExtendedXmlSerialization.ContentModel.Xml
{
	public interface IXmlAttributes
	{
		bool Contains(IIdentity identity);

		bool Next();
	}
}