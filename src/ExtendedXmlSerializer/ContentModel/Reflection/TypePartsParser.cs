using System.Collections.Immutable;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypePartsParser : Parsing<TypeParts>
	{
		readonly static Parser<char> Start = Parse.Char('[')
		                                          .Token(),
		                             Finish = Parse.Char(']')
		                                           .Token();

		public static TypePartsParser Default { get; } = new TypePartsParser();

		TypePartsParser() : this(Identity.Default, TypePartsList.Default, DimensionsParser.Default) {}

		public TypePartsParser(Parser<Key> identity, Parser<ImmutableArray<TypeParts>> arguments,
		                       Parser<ImmutableArray<int>> dimensions)
			: base(
			       identity.SelectMany(arguments.Contained(Start, Finish)
			                                    .Optional())
			               .SelectMany(Parse.Char('^')
			                                .Token()
			                                .Then(dimensions.Accept)
			                                .Optional()
			                                .Accept,
			                           (key, argument)
				                           => new TypeParts(key.Item1.Name, key.Item1.Identifier, key.Item2.Build(),
				                                            argument.GetAssigned()))
			               .ToDelegate()
			               .Cache()
			               .Get()
			      ) {}
	}
}