namespace ExtendedXmlSerializer.ExtensionModel.Encryption
{
	sealed class Encryption : IEncryption
	{
		public static Encryption Default { get; } = new Encryption();

		Encryption() : this(Encrypt.Default, Decrypt.Default) {}

		readonly IEncrypt _encrypt;
		readonly IDecrypt _decrypt;

		public Encryption(IEncrypt encrypt, IDecrypt decrypt)
		{
			_encrypt = encrypt;
			_decrypt = decrypt;
		}

		public string Parse(string data) => _decrypt.Get(data);

		public string Format(string instance) => _encrypt.Get(instance);
	}
}