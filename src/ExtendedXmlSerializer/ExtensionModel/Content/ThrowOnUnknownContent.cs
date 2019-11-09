using System.Xml;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class ThrowOnUnknownContent : ICommand<IFormatReader>
	{
		public static ThrowOnUnknownContent Default { get; } = new ThrowOnUnknownContent();

		ThrowOnUnknownContent() {}

		public void Execute(IFormatReader parameter)
		{
			var line = parameter.Get()
			                    .To<IXmlLineInfo>();

			throw new XmlException($"Unknown/invalid member encountered: '{IdentityFormatter.Default.Get(parameter)}'.",
			                       null,
			                       line.LineNumber, line.LinePosition);
		}
	}
}