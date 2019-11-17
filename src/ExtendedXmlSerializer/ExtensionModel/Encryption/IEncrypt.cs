using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Encryption
{
	/// <summary>
	/// Used to encrypt a string from its plain text input to encrypted output.
	/// </summary>
	public interface IEncrypt : IAlteration<string> {}
}