using System.Reflection;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypeNameFormatter : ITypeFormatter
	{
		public static TypeNameFormatter Default { get; } = new TypeNameFormatter();

		TypeNameFormatter() : this(GenericNameParser.Default, DefaultParsingDelimiters.Default.NestedClass) {}

		readonly Parser<string> _generic;
		readonly string         _nested;

		public TypeNameFormatter(Parser<string> generic, string nested)
		{
			_generic = generic;
			_nested  = nested;
		}

		string Format(TypeInfo type) => type.IsGenericType ? _generic.Parse(type.Name) : type.Name;

		public string Get(TypeInfo parameter)
		{
			var name = Format(parameter);
			var result = parameter.IsNested
				             ? $"{Format(parameter.DeclaringType.GetTypeInfo())}{_nested}{name}"
				             : name;
			return result;
		}
	}
}