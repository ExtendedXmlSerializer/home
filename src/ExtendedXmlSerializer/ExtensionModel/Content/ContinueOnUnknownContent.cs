using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class ContinueOnUnknownContent : ICommand<IFormatReader>
	{
		public static ContinueOnUnknownContent Default { get; } = new ContinueOnUnknownContent();

		ContinueOnUnknownContent() {}

		public void Execute(IFormatReader parameter)
		{
			var reader = parameter.Get().To<System.Xml.XmlReader>();
			var depth  = XmlDepth.Default.Get(reader);

			parameter.Content();

			if (depth.HasValue)
			{
				while (XmlDepth.Default.Get(reader) > depth)
				{
					reader.Skip();
					parameter.Set();
				}
			}
		}
	}
}