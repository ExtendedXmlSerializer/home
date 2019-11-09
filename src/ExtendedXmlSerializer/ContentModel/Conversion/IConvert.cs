namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	public interface IConvert<T>
	{
		T Parse(string data);

		string Format(T instance);
	}
}