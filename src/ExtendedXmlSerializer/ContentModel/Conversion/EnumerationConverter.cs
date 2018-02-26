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

using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class EnumerationConverter : Converter<Enum>
	{
		public EnumerationConverter(Type enumerationType)
			: this(enumerationType,
			       enumerationType.GetTypeInfo()
			                      .GetMembers()
			                      .Where(x => x.MemberType == MemberTypes.Field)
			                      .ToDictionary(x => x.Name, x => x.GetCustomAttribute<XmlEnumAttribute>()
			                                                       ?.Name ?? x.Name)) {}

		public EnumerationConverter(Type enumerationType, IDictionary<string, string> values)
			: base(new Reader(enumerationType, new TableSource<string, string>(values.ToDictionary(x => x.Value, x => x.Key)).Get).Get,
			       new Writer(new TableSource<string, string>(values).Get).Get) {}

		sealed class Reader : IParameterizedSource<string, Enum>
		{
			readonly Type _enumerationType;
			readonly Func<string, string> _values;

			public Reader(Type enumerationType, Func<string, string> values)
			{
				_enumerationType = enumerationType;
				_values = values;
			}

			public Enum Get(string parameter)
				=> parameter != null ? (Enum) Enum.Parse(_enumerationType, _values(parameter)) : default(Enum);
		}

		sealed class Writer : IFormatter<Enum>
		{
			readonly Func<string, string> _source;
			public Writer(Func<string, string> source) => _source = source;

			public string Get(Enum parameter) => _source(parameter.ToString());
		}
	}
}