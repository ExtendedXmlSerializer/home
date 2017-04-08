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

using ExtendedXmlSerializer.Tests.Support;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Conversion
{
	public class ByteArrayConverterTests
	{
		[Fact]
		public void Verify()
		{
			var instance = new byte[] {1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 2, 1};

			var support = new SerializationSupport();
			var actual = support.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><Array xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:item=""unsignedByte"" xmlns=""https://extendedxmlserializer.github.io/system"">AQIDBAUGBwcGBQQDAgE=</Array>");
			Assert.Equal(instance, actual);
		}

		[Fact]
		public void VerifyProperty()
		{
			var instance = new Subject { Bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 2, 1 } };

			var support = new SerializationSupport();
			var actual = support.Cycle(instance);
			Assert.Equal(instance.Bytes, actual.Bytes);
		}

		class Subject
		{
			public byte[] Bytes { get; set; }
		}
	}
}