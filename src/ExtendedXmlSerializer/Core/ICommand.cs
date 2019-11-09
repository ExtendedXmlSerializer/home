namespace ExtendedXmlSerializer.Core
{
	public interface ICommand<in T>
	{
		void Execute(T parameter);
	}
}