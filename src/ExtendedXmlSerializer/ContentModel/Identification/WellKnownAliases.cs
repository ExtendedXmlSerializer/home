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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ExtendedXmlSerializer.ContentModel.Identification
{
	sealed class WellKnownAliases : ReadOnlyDictionary<Type, string>
	{
		public static IReadOnlyDictionary<Type, string> Default { get; } = new WellKnownAliases();

		WellKnownAliases() : base(new Dictionary<Type, string>
		                          {
			                          {typeof(bool), "boolean"},
			                          {typeof(char), "char"},
			                          {typeof(sbyte), "byte"},
			                          {typeof(byte), "unsignedByte"},
			                          {typeof(short), "short"},
			                          {typeof(ushort), "unsignedShort"},
			                          {typeof(int), "int"},
			                          {typeof(uint), "unsignedInt"},
			                          {typeof(long), "long"},
			                          {typeof(ulong), "unsignedLong"},
			                          {typeof(float), "float"},
			                          {typeof(double), "double"},
			                          {typeof(decimal), "decimal"},
			                          {typeof(DateTime), "dateTime"},
			                          {typeof(DateTimeOffset), "dateTimeOffset"},
			                          {typeof(string), "string"},
			                          {typeof(Guid), "guid"},
			                          {typeof(TimeSpan), "TimeSpan"},
			                          {typeof(DictionaryEntry), "Item"}
		                          }) {}
	}
}