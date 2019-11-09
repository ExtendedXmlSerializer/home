using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Parsing;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class FormatReaderContext : IFormatReaderContext
	{
		readonly IIdentityStore      _store;
		readonly IParser<MemberInfo> _parser;

		public FormatReaderContext(IIdentityStore store, IParser<MemberInfo> parser)
		{
			_store  = store;
			_parser = parser;
		}

		public MemberInfo Get(string parameter) => _parser.Get(parameter);

		public IIdentity Get(string name, string identifier) => _store.Get(name, identifier);

		public void Dispose()
		{
			//_disposable.Dispose();
		}
	}
}