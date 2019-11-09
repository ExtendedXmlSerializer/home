using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.ExtensionModel.Expressions;
using Sprache;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class Property : Parsing<KeyValuePair<string, IExpression>>
	{
		public Property(CharacterParser delimiter, Parser<IExpression> expression)
			: this(CodeIdentifier.Default, delimiter, expression) {}

		public Property(Parser<string> name, CharacterParser delimiter, Parser<IExpression> expression)
			: this(name, delimiter.ToParser()
			                      .Token(), expression) {}

		public Property(Parser<string> name, Parser<char> delimiter, Parser<IExpression> expression) : base(
		                                                                                                    name
			                                                                                                    .SelectMany(delimiter.Accept,
			                                                                                                                (s,
			                                                                                                                 _)
				                                                                                                                => s)
			                                                                                                    .SelectMany(expression.Accept,
			                                                                                                                (s,
			                                                                                                                 u)
				                                                                                                                => new
					                                                                                                                KeyValuePair
					                                                                                                                <string
						                                                                                                                , IExpression
					                                                                                                                >(s,
					                                                                                                                  u))
		                                                                                                   ) {}
	}
}