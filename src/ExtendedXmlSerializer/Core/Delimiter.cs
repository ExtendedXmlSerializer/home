using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.Core
{
	struct Delimiter
	{
		readonly char[] _array;
		readonly char   _first;
		readonly string _text;

		public Delimiter(params char[] characters) : this(characters.ToImmutableArray()) {}

		Delimiter(ImmutableArray<char> characters) : this(characters.ToArray(), characters[0]) {}

		Delimiter(char[] array, char first)
		{
			_array = array;
			_first = first;
			_text  = first.ToString();
		}

		public static implicit operator char[](Delimiter delimiter) => delimiter._array;

		public static implicit operator char(Delimiter delimiter) => delimiter._first;

		public static implicit operator Parser<char>(Delimiter delimiter) => Parse.Char(delimiter._first);

		public static implicit operator string(Delimiter delimiter) => delimiter.ToString();

		public override string ToString() => _text;
	}
}