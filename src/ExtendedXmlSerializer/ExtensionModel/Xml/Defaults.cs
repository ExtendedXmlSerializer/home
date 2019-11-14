using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// Convenience class to hold system defaults.
	/// </summary>
	public static class Defaults
	{
		/// <summary>
		/// Default writer settings, which is an unmodified instance of <see cref="XmlWriterSettings"/>.
		/// </summary>
		public static XmlWriterSettings WriterSettings { get; } = new XmlWriterSettings();

		/// <summary>
		/// Default reader settings, which is an instance of <see cref="XmlReaderSettings"/> with its
		/// <see cref="XmlReaderSettings.IgnoreWhitespace"/>, <see cref="XmlReaderSettings.IgnoreComments"/>, and
		/// <see cref="XmlReaderSettings.IgnoreProcessingInstructions"/> all set to true.
		/// </summary>
		public static XmlReaderSettings ReaderSettings { get; } = new XmlReaderSettings
		{
			IgnoreWhitespace             = true,
			IgnoreComments               = true,
			IgnoreProcessingInstructions = true
		};

		internal static XmlReaderSettings CloseRead { get; } = CloseSettings.Default.Get(ReaderSettings);
	}
}