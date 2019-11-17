using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Encryption
{
	/// <summary>
	/// Used to decrypt a string from encrypted input to decrypted output.
	/// </summary>
	public interface IDecrypt : IAlteration<string> {}
}