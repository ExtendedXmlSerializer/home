using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class Prefix : IPrefix
	{
		readonly IPrefix _prefix;

		public Prefix(IPrefix prefix) => _prefix = prefix;

		public string Get(string parameter) => parameter.NullIfEmpty() == null ? string.Empty : _prefix.Get(parameter);
	}
}