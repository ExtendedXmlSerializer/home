using System.Xml;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlParserContexts : ReferenceCache<XmlNameTable, XmlParserContext>, IXmlParserContexts
	{
		public static XmlParserContexts Default { get; } = new XmlParserContexts();

		XmlParserContexts() : base(x => new XmlParserContext(x, new XmlNamespaceManager(x), null, XmlSpace.Default)) {}
	}
}