namespace ExtendedXmlSerializer.Core
{
	sealed class EmptyCommand<T> : ICommand<T>
	{
		public static EmptyCommand<T> Default { get; } = new EmptyCommand<T>();

		EmptyCommand() {}

		public void Execute(T parameter) {}
	}
}