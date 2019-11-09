using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core.Parsing;
using Sprache;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtensionAwareConverter : IConverter
	{
		readonly Parser<MarkupExtensionParts> _parser;
		readonly IConverter                   _converter;

		public MarkupExtensionAwareConverter(IConverter converter) : this(MarkupExtensionParser.Default, converter) {}

		public MarkupExtensionAwareConverter(Parser<MarkupExtensionParts> parser, IConverter converter)
		{
			_parser    = parser;
			_converter = converter;
		}

		public bool IsSatisfiedBy(TypeInfo parameter) => _converter.IsSatisfiedBy(parameter);

		public object Parse(string data) => _parser.ParseAsOptional(data) ?? _converter.Parse(data);

		public string Format(object instance) => _converter.Format(instance);

		public TypeInfo Get() => _converter.Get();
	}
}