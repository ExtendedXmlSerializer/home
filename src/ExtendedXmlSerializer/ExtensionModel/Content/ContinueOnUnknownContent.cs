using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class ContinueOnUnknownContent : ICommand<IFormatReader>
	{
		public static ContinueOnUnknownContent Default { get; } = new ContinueOnUnknownContent();

		ContinueOnUnknownContent() {}

		public void Execute(IFormatReader parameter)
		{
			parameter.Content();
		}
	}
}