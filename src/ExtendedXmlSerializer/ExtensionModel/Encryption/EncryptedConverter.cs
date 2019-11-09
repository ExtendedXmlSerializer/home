using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;

namespace ExtendedXmlSerializer.ExtensionModel.Encryption
{
	sealed class EncryptedConverter : ConverterBase<object>, IConverter
	{
		readonly IEncryption _encryption;
		readonly IConverter  _converter;

		public EncryptedConverter(IEncryption encryption, IConverter converter)
		{
			_encryption = encryption;
			_converter  = converter;
		}

		public override string Format(object instance) => _encryption.Format(_converter.Format(instance));

		public override object Parse(string data) => _converter.Parse(_encryption.Parse(data));

		public TypeInfo Get() => _converter.Get();
	}
}