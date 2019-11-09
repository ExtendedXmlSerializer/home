using System.Collections.Generic;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.Core.Parsing
{
	class Identifier : Parsing<string>
	{
		public Identifier(IEnumerable<char> allowed) : this(allowed.Fixed()) {}

		public Identifier(params char[] allowed)
			: base(Parse.Identifier(Parse.Letter, Parse.LetterOrDigit.XOr(Parse.Chars(allowed)))) {}
	}
}