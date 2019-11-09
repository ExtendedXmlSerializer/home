namespace ExtendedXmlSerializer.Core.Sources
{
	public interface ISource<out T>
	{
		T Get();
	}
}