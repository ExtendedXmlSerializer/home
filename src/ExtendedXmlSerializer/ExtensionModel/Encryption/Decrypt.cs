using System;
using System.Text;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Encryption
{
	class Decrypt : IDecrypt
	{
		public static Decrypt Default { get; } = new Decrypt();

		Decrypt() : this(Encoding.UTF8) {}

		readonly Encoding _encoding;

		[UsedImplicitly]
		public Decrypt(Encoding encoding)
		{
			_encoding = encoding;
		}

		public string Get(string parameter) => _encoding.GetString(Convert.FromBase64String(parameter));
	}
}