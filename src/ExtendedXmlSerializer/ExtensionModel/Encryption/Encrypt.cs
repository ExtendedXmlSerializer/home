using System;
using System.Text;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Encryption
{
	class Encrypt : IEncrypt
	{
		public static Encrypt Default { get; } = new Encrypt();

		Encrypt() : this(Encoding.UTF8) {}

		readonly Encoding _encoding;

		[UsedImplicitly]
		public Encrypt(Encoding encoding)
		{
			_encoding = encoding;
		}

		public string Get(string parameter) => Convert.ToBase64String(_encoding.GetBytes(parameter));
	}
}