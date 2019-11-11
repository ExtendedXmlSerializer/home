using ExtendedXmlSerializer.Core.Sources;
using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class CloseSettings : IAlteration<XmlWriterSettings>, IAlteration<XmlReaderSettings>
	{
		public static CloseSettings Default { get; } = new CloseSettings();

		CloseSettings() {}

		public XmlWriterSettings Get(XmlWriterSettings parameter)
		{
			var result = parameter.Clone();
			result.CloseOutput = true;
			return result;
		}

		public XmlReaderSettings Get(XmlReaderSettings parameter)
		{
			var result = parameter.Clone();
			result.CloseInput = true;
			return result;
		}
	}
}