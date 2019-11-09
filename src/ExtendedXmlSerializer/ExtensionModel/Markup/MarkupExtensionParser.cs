using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Parsing;
using Sprache;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtensionParser : Parsing<MarkupExtensionParts>
	{
		public static MarkupExtensionParser Default { get; } = new MarkupExtensionParser();

		MarkupExtensionParser() : base(MarkupExtensionExpression.Default.ToParser()
		                                                        .End()
		                                                        .ToDelegate()
		                                                        .Cache()
		                                                        .Get()) {}
	}
}