using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.Samples.Introduction
{
	public sealed class Run : ICommand<object>
	{
		public static Run Default { get; } = new Run();
		Run() {}

		public void Execute(object parameter)
		{
			Create.Default.Execute(null);
			Type.Default.Execute(null);
			Member.Default.Execute(null);
		}
	}
}