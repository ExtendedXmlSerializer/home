using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Parsing;
using Sprache;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class GenericNameParser : Parsing<string>
	{
		public static GenericNameParser Default { get; } = new GenericNameParser();

		GenericNameParser() : this(CodeIdentifier.Default, Parse.Char(DefaultClrDelimiters.Default.Generic)) {}

		public GenericNameParser(Parser<string> identifier, Parser<char> delimiter) : base(
		                                                                                   identifier
			                                                                                   .SelectMany(delimiter
			                                                                                               .Optional()
			                                                                                               .Accept,
			                                                                                               (name, _)
				                                                                                               => name)
		                                                                                  ) {}
	}
}