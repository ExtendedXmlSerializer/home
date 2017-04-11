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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class TypeArguments : IParameterizedSource<IFormatReader, ImmutableArray<Type>>,
	                             IFormattedContent<ImmutableArray<Type>>
	{
		public static TypeArguments Default { get; } = new TypeArguments();
		TypeArguments() : this(TypePartsList.Default, TypePartsFormatter.Default.Get) {}

		readonly Parser<ImmutableArray<TypeParts>> _list;
		readonly Func<TypeParts, string> _formatter;

		public TypeArguments(Parser<ImmutableArray<TypeParts>> list, Func<TypeParts, string> formatter)
		{
			_list = list;
			_formatter = formatter;
		}

		public ImmutableArray<Type> Get(IFormatReader parameter) => _list.Parse(parameter.Content())
		                                                                 .Select(_formatter)
		                                                                 .Select(parameter.Get)
		                                                                 .Cast<TypeInfo>()
		                                                                 .Select(x => x.AsType())
		                                                                 .ToImmutableArray();

		public string Get(IFormatWriter writer, ImmutableArray<Type> instance) => string.Join(",", Pack(writer, instance));

		static string[] Pack(IFormatWriter writer, ImmutableArray<Type> arguments)
		{
			var length = arguments.Length;
			var result = new string[length];
			for (var i = 0; i < length; i++)
			{
				result[i] = writer.Get(arguments[i].GetTypeInfo());
			}
			return result;
		}
	}
}