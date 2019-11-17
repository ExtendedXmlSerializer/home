using ExtendedXmlSerializer.ContentModel.Conversion;

namespace ExtendedXmlSerializer.ExtensionModel.Encryption
{
	/// <summary>
	/// A converter used to encrypt and decrypt values.
	/// </summary>
	public interface IEncryption : IConvert<string> {}
}