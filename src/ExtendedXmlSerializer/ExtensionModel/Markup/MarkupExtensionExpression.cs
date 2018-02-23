// MIT License
// 
// Copyright (c) 2016-2018 Wojciech Nagórski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.Core.Sprache;
using ExtendedXmlSerializer.ExtensionModel.Expressions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtensionExpression : Parsing<MarkupExtensionParts>
	{
		readonly static CharacterParser Start = '{',
			Finish = '}',
			PropertyDelimiter = '=',
			Item = Core.Sources.Defaults.ItemDelimiter;

		readonly static Tuple<ImmutableArray<IExpression>, ImmutableArray<KeyValuePair<string, IExpression>>>
			DefaultValue = Tuple.Create(ImmutableArray<IExpression>.Empty,
			                            ImmutableArray<KeyValuePair<string, IExpression>>.Empty);

		readonly static Parser<IExpression> Expression =
			Parse.Ref(() => Default.ToParser()).Select(x => (IExpression) new MarkupExtensionPartsExpression(x))
			     .XOr(new Expression(Item.Character, Finish.Character));

		public static MarkupExtensionExpression Default { get; } = new MarkupExtensionExpression();
		MarkupExtensionExpression() : this(Item, Expression, new Property(PropertyDelimiter, Expression)) {}

		public MarkupExtensionExpression(Parser<char> item, Parser<IExpression> argument,
		                                 Parser<KeyValuePair<string, IExpression>> property)
			: this(Start, TypePartsParser.Default, item, new ItemsParser<IExpression>(property.Not().Then(argument.Accept)),
			       new ItemsParser<KeyValuePair<string, IExpression>>(property), Finish) {}

		public MarkupExtensionExpression(Parser<char> start, Parser<TypeParts> typeParts, Parser<char> item,
		                                 Parser<ImmutableArray<IExpression>> arguments,
		                                 Parser<ImmutableArray<KeyValuePair<string, IExpression>>> properties,
		                                 Parser<char> finish)
			: base(typeParts.SelectMany(arguments.Optional()
			                                     .SelectMany(item.Then(properties.Accept).XOptional()
			                                                     .Or(properties.Optional()))
			                                     .Token()
			                                     .Accept,
			                            (type, option) =>
			                            {
				                            var result = new MarkupExtensionParts(
					                            type,
					                            option.Item1.GetOrElse(DefaultValue.Item1),
					                            option.Item2.GetOrElse(DefaultValue.Item2)
				                            );
				                            return result;
			                            }
			                )
			                .Contained(start, finish.Token())) {}
	}
}