using System;
using System.Linq;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class TypePartsFormatter : ITypePartsFormatter
	{
		public static TypePartsFormatter Default { get; } = new TypePartsFormatter();

		TypePartsFormatter() : this(IdentityFormatter<TypeParts>.Default) {}

		readonly IFormatter<TypeParts>   _formatter;
		readonly Func<TypeParts, string> _selector;

		public TypePartsFormatter(IFormatter<TypeParts> formatter)
		{
			_formatter = formatter;
			_selector  = Get;
		}

		public string Get(TypeParts parameter)
		{
			var parts     = parameter.GetArguments();
			var arguments = parts.HasValue ? $"[{string.Join(",", parts.Value.Select(_selector))}]" : null;
			var dimensions = parameter.Dimensions.HasValue
				                 ? $"^{string.Join(",", parameter.Dimensions.Value.ToArray())}"
				                 : null;
			var result = $"{_formatter.Get(parameter)}{arguments}{dimensions}";
			return result;
		}
	}
}