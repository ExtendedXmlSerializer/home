using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypeNameAlteration : IAlteration<string>
	{
		readonly char _search;
		readonly char _replace;

		public static TypeNameAlteration Default { get; } = new TypeNameAlteration();

		TypeNameAlteration() : this(DefaultParsingDelimiters.Default.NestedClass,
		                            DefaultClrDelimiters.Default.NestedClass) {}

		public TypeNameAlteration(char search, char replace)
		{
			_search  = search;
			_replace = replace;
		}

		public string Get(string parameter) => parameter.Replace(_search, _replace);
	}
}