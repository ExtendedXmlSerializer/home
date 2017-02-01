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

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public class KnownElements : IEnumerable<IElement>
	{
		public virtual IEnumerator<IElement> GetEnumerator()
		{
			yield return new Element("boolean", typeof(bool));
			yield return new Element("char", typeof(char));
			yield return new Element("byte", typeof(sbyte));
			yield return new Element("unsignedByte", typeof(byte));
			yield return new Element("short", typeof(short));
			yield return new Element("unsignedShort", typeof(ushort));
			yield return new Element("int", typeof(int));
			yield return new Element("unsignedInt", typeof(uint));
			yield return new Element("long", typeof(long));
			yield return new Element("unsignedLong", typeof(ulong));
			yield return new Element("float", typeof(float));
			yield return new Element("double", typeof(double));
			yield return new Element("decimal", typeof(decimal));
			yield return new Element("dateTime", typeof(DateTime));
			yield return new Element("dateTimeOffset", typeof(DateTimeOffset));
			yield return new Element("string", typeof(string));
			yield return new Element("guid", typeof(Guid));
			yield return new Element("TimeSpan", typeof(TimeSpan));
			yield return new Element("Item", typeof(DictionaryEntry));
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}