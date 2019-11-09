using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
// ReSharper disable TooManyDependencies

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class FormatWriters : IFormatWriters<System.Xml.XmlWriter>
	{
		readonly static Aliases              Aliases = Aliases.Default;
		readonly        IIdentifierFormatter _formatter;
		readonly        ITypePartResolver    _parts;
		readonly        IPrefixes            _prefixes;
		readonly        IIdentityStore       _store;

		readonly IAliases _table;

		public FormatWriters(IIdentifierFormatter formatter, IIdentityStore store, ITypePartResolver parts,
		                     IPrefixes prefixes)
			: this(Aliases, formatter, store, parts, prefixes) {}

		public FormatWriters(IAliases table, IIdentifierFormatter formatter, IIdentityStore store,
		                     ITypePartResolver parts, IPrefixes prefixes)
		{
			_table     = table;
			_formatter = formatter;
			_store     = store;
			_parts     = parts;
			_prefixes  = prefixes;
		}

		public IFormatWriter Get(System.Xml.XmlWriter parameter)
			=> new XmlWriter(_table, _formatter, _store, _parts, _prefixes.Get(parameter), parameter);
	}
}