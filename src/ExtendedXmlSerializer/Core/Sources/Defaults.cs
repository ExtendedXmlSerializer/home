using ExtendedXmlSerializer.Core.Parsing;
using Sprache;

namespace ExtendedXmlSerializer.Core.Sources
{
	static class Defaults
	{
		public static CharacterParser ItemDelimiter { get; } = new CharacterParser(',', Parse.Char(',')
		                                                                                     .Token());
	}
}