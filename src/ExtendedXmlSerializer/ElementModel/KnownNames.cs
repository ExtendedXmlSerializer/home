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
using System.Collections;
using System.Collections.Generic;

namespace ExtendedXmlSerialization.ElementModel
{
	public class KnownNames : IEnumerable<IElementName>
	{
		public virtual IEnumerator<IElementName> GetEnumerator()
		{
			yield return new ElementName(typeof(bool), "boolean");
			yield return new ElementName(typeof(char), "char");
			yield return new ElementName(typeof(sbyte), "byte");
			yield return new ElementName(typeof(byte), "unsignedByte");
			yield return new ElementName(typeof(short), "short");
			yield return new ElementName(typeof(ushort), "unsignedShort");
			yield return new ElementName(typeof(int), "int");
			yield return new ElementName(typeof(uint), "unsignedInt");
			yield return new ElementName(typeof(long), "long");
			yield return new ElementName(typeof(ulong), "unsignedLong");
			yield return new ElementName(typeof(float), "float");
			yield return new ElementName(typeof(double), "double");
			yield return new ElementName(typeof(decimal), "decimal");
			yield return new ElementName(typeof(DateTime), "dateTime");
			yield return new ElementName(typeof(DateTimeOffset), "dateTimeOffset");
			yield return new ElementName(typeof(string), "string");
			yield return new ElementName(typeof(Guid), "guid");
			yield return new ElementName(typeof(TimeSpan), "TimeSpan");
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}