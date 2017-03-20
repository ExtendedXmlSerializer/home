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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ExtendedXmlSerializer.Tests.Support;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Collections
{
	public class CollectionContentOptionTests
	{
		[Fact]
		public void Field()
		{
			var support = new SerializationSupport();
			var expected = new Subject("Hello", "World!");
			var actual = support.Assert(expected, @"<?xml version=""1.0"" encoding=""utf-8""?><CollectionContentOptionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ContentModel.Collections;assembly=ExtendedXmlSerializer.Tests""><_children><Capacity>4</Capacity><string xmlns=""https://github.com/wojtpl2/ExtendedXmlSerializer/system"">Hello</string><string xmlns=""https://github.com/wojtpl2/ExtendedXmlSerializer/system"">World!</string></_children></CollectionContentOptionTests-Subject>");
			Assert.Equal(expected.ToArray(), actual.ToArray());
		}

		class Subject : IEnumerable<string>
		{
			[XmlElement] readonly List<string> _children = new List<string>();

			public Subject(params string[] items)
			{
				_children.AddRange(items);
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public IEnumerator<string> GetEnumerator() => _children.GetEnumerator();
		}
	}
}