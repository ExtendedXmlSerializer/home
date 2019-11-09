using System;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Parsing;
using Sprache;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class Identity : Parsing<Key>
	{
		readonly static Func<string, Parser<char>> Separator = Parse.Char(':')
		                                                            .Accept;

		public static Identity Default { get; } = new Identity();

		Identity() : this(Identifier.Default) {}

		public Identity(Parser<string> identifier)
			: base(
			       identifier.SelectMany(Separator, (prefix, _) => prefix)
			                 .Optional()
			                 .SelectMany(identifier.Accept,
			                             (option, s) => option.IsDefined ? new Key(s, option.Get()) : new Key(s)
			                            )
			      ) {}
	}
}