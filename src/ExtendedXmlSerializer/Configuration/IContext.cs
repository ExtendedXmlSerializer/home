namespace ExtendedXmlSerializer.Configuration
{
	public interface IContext
	{
		IRootContext Root { get; }

		IContext Parent { get; }
	}
}