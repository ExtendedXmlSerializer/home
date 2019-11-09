using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	sealed class SetContentCommand : ICommand<IFormatReader>
	{
		public static SetContentCommand Default { get; } = new SetContentCommand();

		SetContentCommand() {}

		public void Execute(IFormatReader parameter) => parameter.Set();
	}
}