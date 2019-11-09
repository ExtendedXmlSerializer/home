using System.Xml;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class IdentityMapper : IIdentityStore
	{
		readonly IIdentityStore        _store;
		readonly IXmlNamespaceResolver _manager;

		public IdentityMapper(IIdentityStore store, IXmlNamespaceResolver manager)
		{
			_store   = store;
			_manager = manager;
		}

		public IIdentity Get(string name, string identifier) => _store.Get(name, _manager.LookupNamespace(identifier));
	}
}