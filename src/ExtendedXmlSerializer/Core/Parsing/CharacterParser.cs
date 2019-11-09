using Sprache;

namespace ExtendedXmlSerializer.Core.Parsing
{
	sealed class CharacterParser : Parsing<char>
	{
		public static implicit operator CharacterParser(char instance) => new CharacterParser(instance);

		public CharacterParser(char character) : this(character, Parse.Char(character)) {}

		public CharacterParser(char character, Parser<char> parser) : base(parser)
		{
			Character = character;
		}

		public char Character { get; }
	}
}