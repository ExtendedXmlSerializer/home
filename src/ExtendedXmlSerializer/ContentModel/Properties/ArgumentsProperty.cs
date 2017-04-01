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
using ExtendedXmlSerializer.ContentModel.Conversion.Formatting;
using ExtendedXmlSerializer.ContentModel.Conversion.Parsing;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Sprache;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class ArgumentsProperty : FrameworkPropertyBase<ImmutableArray<Type>>, IArgumentsProperty
	{
		[UsedImplicitly]
		public ArgumentsProperty(IReflectionParsers parsers, IReflectionFormatters formatters)
			: this(TypePartsList.Default, parsers, formatters) {}

		readonly Parser<ImmutableArray<TypeParts>> _names;
		readonly IParameterizedSource<IXmlReader, IReflectionParser> _parsers;
		readonly IParameterizedSource<IXmlWriter, IReflectionFormatter> _formatters;

		ArgumentsProperty(Parser<ImmutableArray<TypeParts>> names, IParameterizedSource<IXmlReader, IReflectionParser> parsers,
		                  IParameterizedSource<IXmlWriter, IReflectionFormatter> formatters) : base("arguments")
		{
			_names = names;
			_parsers = parsers;
			_formatters = formatters;
		}

		protected override ImmutableArray<Type> Parse(IXmlReader parameter, string data)
		{
			var items = _names.Parse(data).ToArray();
			var result = Unpack(parameter, items).ToImmutableArray();
			return result;
		}

		Type[] Unpack(IXmlReader reader, params TypeParts[] parts)
		{
			var parser = _parsers.Get(reader);
			var length = parts.Length;
			var result = new Type[length];
			for (var i = 0; i < length; i++)
			{
				var typeInfo = parser.Get(parts[i]);
				result[i] = typeInfo.AsType();
			}
			return result;
		}

		protected override string Format(IXmlWriter writer, ImmutableArray<Type> instance)
			=> string.Join(",", Pack(writer, instance));

		string[] Pack(IXmlWriter writer, ImmutableArray<Type> arguments)
		{
			var formatter = _formatters.Get(writer);
			var length = arguments.Length;
			var result = new string[length];
			for (var i = 0; i < length; i++)
			{
				result[i] = formatter.Get(arguments[i].GetTypeInfo());
			}
			return result;
		}
	}
}