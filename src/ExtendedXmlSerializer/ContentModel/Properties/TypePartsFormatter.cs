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

using System;
using System.Linq;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class TypePartsFormatter : ITypePartsFormatter
	{
		public static TypePartsFormatter Default { get; } = new TypePartsFormatter();
		TypePartsFormatter() : this(IdentityFormatter<TypeParts>.Default) {}

		readonly IFormatter<TypeParts> _formatter;
		readonly Func<TypeParts, string> _selector;

		public TypePartsFormatter(IFormatter<TypeParts> formatter)
		{
			_formatter = formatter;
			_selector = Get;
		}

		public string Get(TypeParts parameter)
		{
			var arguments = parameter.GetArguments();
			var append = arguments.HasValue ? $"[{string.Join(",", arguments.Value.Select(_selector))}]" : null;
			var result = $"{_formatter.Get(parameter)}{append}";
			return result;
		}
	}
}