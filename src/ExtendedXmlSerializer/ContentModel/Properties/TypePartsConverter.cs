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
using System.Linq;
using ExtendedXmlSerializer.ContentModel.Converters;
using ExtendedXmlSerializer.ContentModel.Formatting;
using ExtendedXmlSerializer.ContentModel.Parsing;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class TypePartsConverter : ConverterBase<TypeParts>, ITypePartsConverter
	{
		public static TypePartsConverter Default { get; } = new TypePartsConverter();
		TypePartsConverter() : this(IdentityFormatter<TypeParts>.Default, TypePartsParser.Default) {}

		readonly IFormatter<TypeParts> _formatter;
		readonly Parser<TypeParts> _parts;
		readonly Func<TypeParts, string> _selector;

		public TypePartsConverter(IFormatter<TypeParts> formatter, Parser<TypeParts> parts)
		{
			_formatter = formatter;
			_parts = parts;
			_selector = Format;
		}

		public override TypeParts Parse(string data) => _parts.Parse(data);

		public override string Format(TypeParts instance)
		{
			var arguments = instance.GetArguments();
			var append = arguments.HasValue ? $"[{string.Join(",", arguments.Value.Select(_selector))}]" : null;
			var result = $"{_formatter.Get(instance)}{append}";
			return result;
		}
	}
}