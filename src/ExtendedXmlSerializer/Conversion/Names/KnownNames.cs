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

namespace ExtendedXmlSerialization.ElementModel.Names
{
	public class KnownNames : IEnumerable<IName>
	{
		public virtual IEnumerator<IName> GetEnumerator()
		{
			yield return new Name("boolean", typeof(bool));
			yield return new Name("char", typeof(char));
			yield return new Name("byte", typeof(sbyte));
			yield return new Name("unsignedByte", typeof(byte));
			yield return new Name("short", typeof(short));
			yield return new Name("unsignedShort", typeof(ushort));
			yield return new Name("int", typeof(int));
			yield return new Name("unsignedInt", typeof(uint));
			yield return new Name("long", typeof(long));
			yield return new Name("unsignedLong", typeof(ulong));
			yield return new Name("float", typeof(float));
			yield return new Name("double", typeof(double));
			yield return new Name("decimal", typeof(decimal));
			yield return new Name("dateTime", typeof(DateTime));
			yield return new Name("dateTimeOffset", typeof(DateTimeOffset));
			yield return new Name("string", typeof(string));
			yield return new Name("guid", typeof(Guid));
			yield return new Name("TimeSpan", typeof(TimeSpan));
			yield return new Name("Item", typeof(DictionaryEntry));
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}