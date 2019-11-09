using System.Collections.Generic;
using System.Collections.Immutable;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ExtensionModel.Expressions;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtensionParts
	{
		public MarkupExtensionParts(TypeParts type, ImmutableArray<IExpression> arguments,
		                            ImmutableArray<KeyValuePair<string, IExpression>> properties)
		{
			Type       = type;
			Arguments  = arguments;
			Properties = properties;
		}

		public TypeParts Type { get; }
		public ImmutableArray<IExpression> Arguments { get; }
		public ImmutableArray<KeyValuePair<string, IExpression>> Properties { get; }
	}
}