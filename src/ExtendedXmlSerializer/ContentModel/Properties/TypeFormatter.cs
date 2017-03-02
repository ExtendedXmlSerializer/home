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
using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.ContentModel.Xml.Parsing;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Properties
{
	sealed class TypeFormatter : CacheBase<TypeInfo, string>, ITypeFormatter
	{
		readonly static Xml.Identities Identities = Xml.Identities.Default;
		readonly static NameConverter Converter = NameConverter.Default;
		public static IParameterizedSource<IXmlWriter, ITypeFormatter> Defaults { get; } =
			new ReferenceCache<IXmlWriter, ITypeFormatter>(x => new TypeFormatter(x));

		readonly IXmlWriter _writer;
		readonly Xml.IIdentities _identities;
		readonly INameConverter _converter;

		TypeFormatter(IXmlWriter writer) : this(writer, Identities, Converter) {}

		public TypeFormatter(IXmlWriter writer, Xml.IIdentities identities, INameConverter converter)
		{
			_writer = writer;
			_identities = identities;
			_converter = converter;
		}

		protected override string Create(TypeInfo parameter) => _converter.Format(Name(parameter));

		ParsedName Name(TypeInfo parameter)
		{
			var identity = _identities.Get(parameter);
			var result = new ParsedName(identity.Name, _writer.Get(identity.Identifier),
			                            parameter.IsGenericType ? Arguments(parameter.GetGenericArguments()) : null);
			return result;
		}

		Func<ImmutableArray<ParsedName>> Arguments(Type[] types)
		{
			var length = types.Length;
			var names = new ParsedName[length];
			for (var i = 0; i < length; i++)
			{
				names[i] = Name(types[i].GetTypeInfo());
			}
			var result = new Func<ImmutableArray<ParsedName>>(names.ToImmutableArray);
			return result;
		}
	}
}