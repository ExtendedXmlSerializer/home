namespace ExtendedXmlSerializer.ContentModel.Format
{
	interface IFormattedContent<in T>
	{
		string Get(IFormatWriter writer, T instance);
	}
}