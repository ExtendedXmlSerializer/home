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

using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class TextLiteral : FixedParser<string>
	{
		const char Slash = '\\';

		readonly static Parser<char> EscapedCharacter = Parse.Char(Slash).Then(Parse.CharExcept(Slash).Accept);

		public TextLiteral(char containingCharacter) : this(containingCharacter, Parse.Char(containingCharacter)) {}

		public TextLiteral(char containingCharacter, Parser<char> container) : base(
			new EscapedLiteral(containingCharacter).Get().XOr(
				                                       Parse.CharExcept($"{containingCharacter}{Slash}")
				                                            .Or(EscapedCharacter)
				                                            .Many()
				                                            .Text()
			                                       ).Contained(container, container)
			                                       .Select(x => x.Quoted())
			                                       .Token()
		) {}
	}
}