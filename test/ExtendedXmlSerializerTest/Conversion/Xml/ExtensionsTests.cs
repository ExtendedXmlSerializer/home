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
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace ExtendedXmlSerialization.Test.Conversion.Xml
{
	public class ExtensionsTests
	{
		/*readonly ITestOutputHelper _output;
		public ExtensionsTests(ITestOutputHelper output)
		{
			_output = output;
		}*/

		/*[Fact]
		public void FinishCurrentElement()
		{
			const string
				Child = @"<Test att=""first""><Second att=""second""/></Test>",
				Children = @"<Test att=""first""><Second att=""second""/><Second att=""second""/></Test>",
				Open = @"<Test att=""first""></Test>",
				Closed = @"<Test att=""first"" />";

			using (var reader = XmlReader.Create(new StringReader(Children)))
			{
				while (reader.Read())
				{
					_output.WriteLine($"{reader.Depth}: {reader.IsEmptyElement} - {reader.IsStartElement()} {reader.NodeType}");
				}
			}

			throw new InvalidOperationException();
		}*/
	}
}