using System.Xml;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class FormatReaderContexts
		: ReferenceCacheBase<System.Xml.XmlReader, IFormatReaderContext>,
		  IFormatReaderContexts
	{
		readonly IIdentityStore     _store;
		readonly IXmlParserContexts _contexts;
		readonly ITypes             _types;

		public FormatReaderContexts(IIdentityStore store, ITypes types) :
			this(store, types, XmlParserContexts.Default) {}

		public FormatReaderContexts(IIdentityStore store, ITypes types, IXmlParserContexts contexts)
		{
			_store    = store;
			_contexts = contexts;
			_types    = types;
		}

		static XmlNamespaceManager Default(System.Xml.XmlReader parameter)
			=> new XmlNamespaceManager(parameter.NameTable ?? parameter.Settings.NameTable);

		protected override IFormatReaderContext Create(System.Xml.XmlReader parameter)
		{
			var resolver = _contexts.IsSatisfiedBy(parameter.NameTable)
				               ? _contexts.Get(parameter.NameTable)
				                          .NamespaceManager
				               : parameter as IXmlNamespaceResolver ?? Default(parameter);
			var mapper = new IdentityMapper(_store, resolver);

			var reflector = new TypePartReflector(mapper, _types);
			var types     = new TypeParser(reflector);
			var parser    = new ReflectionParser(types, reflector);
			var result    = new FormatReaderContext(mapper, parser);
			return result;
		}
	}
}