using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Encryption
{
	sealed class EncryptionConverterAlteration : IAlteration<IConverter>
	{
		public static EncryptionConverterAlteration Default { get; } = new EncryptionConverterAlteration();

		EncryptionConverterAlteration() : this(Encryption.Default) {}

		readonly IEncryption _encryption;

		public EncryptionConverterAlteration(IEncryption encryption) => _encryption = encryption;

		public IConverter Get(IConverter parameter) => new EncryptedConverter(_encryption, parameter);
	}
}