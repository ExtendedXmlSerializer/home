// MIT License
//
// Copyright (c) 2016 Wojciech Nagórski
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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ExtendedXmlSerializer.ContentModel.Parsing;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Sprache;
using ExtendedXmlSerializer.ExtensionModel.Expressions;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtensionParser : Parsing<MarkupExtensionParts>
	{
		readonly static CharacterParser Start = '{',
			Finish = '}',
			PropertyDelimiter = '=',
			Item = Core.Sources.Defaults.ItemDelimiter;

		readonly static Tuple<ImmutableArray<IExpression>, ImmutableArray<KeyValuePair<string, IExpression>>>
			DefaultValue = Tuple.Create(ImmutableArray<IExpression>.Empty,
			                            ImmutableArray<KeyValuePair<string, IExpression>>.Empty);

		readonly static Expression Expression = new Expression(Item.Character, Finish.Character);

		readonly static Parser<Tuple<ImmutableArray<IExpression>, ImmutableArray<KeyValuePair<string, IExpression>>>>
			DefaultReturn = Parse.String(string.Empty).Return(DefaultValue);

		public static MarkupExtensionParser Default { get; } = new MarkupExtensionParser();
		MarkupExtensionParser() : this(Item, Expression, new Property(PropertyDelimiter, Expression)) {}

		public MarkupExtensionParser(Parser<char> item, Parser<IExpression> argument,
		                             Parser<KeyValuePair<string, IExpression>> property)
			: this(Start, TypePartsParser.Default, item, new ItemsParser<IExpression>(property.Not().Then(argument.Accept)),
			       new ItemsParser<KeyValuePair<string, IExpression>>(property), Finish) {}

		public MarkupExtensionParser(Parser<char> start, Parser<TypeParts> typeParts, Parser<char> item,
		                             Parser<ImmutableArray<IExpression>> arguments,
		                             Parser<ImmutableArray<KeyValuePair<string, IExpression>>> properties, Parser<char> finish)
			: base(
				typeParts.SelectMany(
					         Parse.WhiteSpace.AtLeastOnce().Then(
						         arguments.SelectMany(item.Accept, (array, _) => array).SelectMany(properties)
						                  .Or(properties.Select(dictionary => Tuple.Create(DefaultValue.Item1, dictionary)))
						                  .Or(arguments.Select(array => Tuple.Create(array, DefaultValue.Item2)))
						                  .Or(DefaultReturn)
						                  .Accept
					         ).Optional().Accept,
					         (type, option) =>
					         {
						         var contents = option.GetOrElse(DefaultValue);
						         var result = new MarkupExtensionParts(type, contents.Item1, contents.Item2);
						         return result;
					         }
				         )
				         .Contained(start, finish)
				         .ToDelegate()
				         .Cache()
				         .ToParser()) {}
	}
}