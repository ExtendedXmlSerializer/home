using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.ExtensionModel.Expressions;
using Sprache;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtensionExpression : Parsing<MarkupExtensionParts>
	{
		readonly static CharacterParser Start             = '{',
		                                Finish            = '}',
		                                PropertyDelimiter = '=',
		                                Item              = Core.Sources.Defaults.ItemDelimiter;

		readonly static Tuple<ImmutableArray<IExpression>, ImmutableArray<KeyValuePair<string, IExpression>>>
			DefaultValue = Tuple.Create(ImmutableArray<IExpression>.Empty,
			                            ImmutableArray<KeyValuePair<string, IExpression>>.Empty);

		readonly static Parser<IExpression> Expression =
			Parse.Ref(() => Default.ToParser())
			     .Select(x => (IExpression)new MarkupExtensionPartsExpression(x))
			     .XOr(new Expression(Item.Character, Finish.Character));

		public static MarkupExtensionExpression Default { get; } = new MarkupExtensionExpression();

		MarkupExtensionExpression() : this(Item, Expression, new Property(PropertyDelimiter, Expression)) {}

		public MarkupExtensionExpression(Parser<char> item, Parser<IExpression> argument,
		                                 Parser<KeyValuePair<string, IExpression>> property)
			: this(Start, TypePartsParser.Default, item, new ItemsParser<IExpression>(property.Not()
			                                                                                  .Then(argument.Accept)),
			       new ItemsParser<KeyValuePair<string, IExpression>>(property), Finish) {}

		// ReSharper disable once TooManyDependencies
		public MarkupExtensionExpression(Parser<char> start, Parser<TypeParts> typeParts, Parser<char> item,
		                                 Parser<ImmutableArray<IExpression>> arguments,
		                                 Parser<ImmutableArray<KeyValuePair<string, IExpression>>> properties,
		                                 Parser<char> finish)
			: base(typeParts.SelectMany(arguments.Optional()
			                                     .SelectMany(item.Then(properties.Accept)
			                                                     .XOptional()
			                                                     .Or(properties.Optional()))
			                                     .Token()
			                                     .Accept,
			                            (type, option) =>
			                            {
				                            var result = new MarkupExtensionParts(
				                                                                  type,
				                                                                  option.Item1.GetOrElse(DefaultValue
					                                                                                         .Item1),
				                                                                  option.Item2.GetOrElse(DefaultValue
					                                                                                         .Item2)
				                                                                 );
				                            return result;
			                            }
			                           )
			                .Contained(start, finish.Token())) {}
	}
}