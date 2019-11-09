using ExtendedXmlSerializer.ExtensionModel.Expressions;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtensionPartsExpression : FixedExpression<MarkupExtensionParts>
	{
		public MarkupExtensionPartsExpression(MarkupExtensionParts parts) : base(parts) {}
	}
}