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
using ExtendedXmlSerializer.Tests.TestObject;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Members
{
	public class MemberSerializationsTests
	{
		[Fact]
		public void XmlElementAttribute()
		{
			var expected = new TestClassWithXmlElementAttribute {Id = 123};
			var actual = new SerializationSupport().Assert(
				expected,
				@"<?xml version=""1.0"" encoding=""utf-8""?><TestClassWithXmlElementAttribute xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Identifier>123</Identifier></TestClassWithXmlElementAttribute>"
			);
			Assert.Equal(expected.Id, actual.Id);
		}

		[Fact]
		public void XmlElementWithOrder()
		{
			var expected = new TestClassWithOrderParameters {A = "A", B = "B"};
			var actual = new SerializationSupport().Assert(
				expected,
				@"<?xml version=""1.0"" encoding=""utf-8""?><TestClassWithOrderParameters xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><A>A</A><B>B</B></TestClassWithOrderParameters>"
			);
			Assert.Equal(expected.A, actual.A);
			Assert.Equal(expected.B, actual.B);
		}

		[Fact]
		public void XmlElementWithOrderExt()
		{
			var expected = new TestClassWithOrderParametersExt {A = "A", B = "B", C = "C", D = "D"};
			var actual = new SerializationSupport().Assert(
				expected,
				@"<?xml version=""1.0"" encoding=""utf-8""?><TestClassWithOrderParametersExt xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><A>A</A><B>B</B><D>D</D><C>C</C></TestClassWithOrderParametersExt>"
			);
			Assert.Equal(expected.A, actual.A);
			Assert.Equal(expected.B, actual.B);
			Assert.Equal(expected.C, actual.C);
			Assert.Equal(expected.D, actual.D);
		}

		[Fact]
		public void TestClassInheritanceWithOrder()
		{
			var expected = TestObject.TestClassInheritanceWithOrder.Create();
			var actual = new SerializationSupport().Assert(
				expected,
				@"<?xml version=""1.0"" encoding=""utf-8""?><TestClassInheritanceWithOrder xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Id2>3</Id2><Id>2</Id></TestClassInheritanceWithOrder>"
			);
			Assert.Equal(expected.Id2, actual.Id2);
			Assert.Equal(expected.Id, actual.Id);
		}

		[Fact]
		public void TestClassInterfaceInheritanceWithOrder()
		{
			var expected = new TestClassInterfaceInheritanceWithOrder {Id = 1, Id2 = 2};
			var actual = new SerializationSupport().Assert(
				expected,
				@"<?xml version=""1.0"" encoding=""utf-8""?><TestClassInterfaceInheritanceWithOrder xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Id2>2</Id2><Id>1</Id></TestClassInterfaceInheritanceWithOrder>"
			);
			Assert.Equal(expected.Id2, actual.Id2);
			Assert.Equal(expected.Id, actual.Id);
		}

		[Fact]
		public void XmlRoot()
		{
			var expected = new TestClassWithXmlRootAttribute {Id = 123};
			var actual = new SerializationSupport().Assert(
				expected,
				@"<?xml version=""1.0"" encoding=""utf-8""?><TestClass xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Id>123</Id></TestClass>"
			);
			Assert.Equal(expected.Id, actual.Id);
		}
	}
}