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

using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test.Legacy
{
	public class SerializationWithXmlAttributes : BaseTest
	{
		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void XmlElement()
		{
			var obj = new TestClassWithXmlElementAttribute {Id = 123};

			CheckSerializationAndDeserializationByXml(
				@"<TestClassWithXmlElementAttribute type=""ExtendedXmlSerialization.Test.TestObject.TestClassWithXmlElementAttribute"">
  <Identifier>123</Identifier>
</TestClassWithXmlElementAttribute>", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void XmlElementWithOrder()
		{
			var obj = new TestClassWithOrderParameters {A = "A", B = "B"};
			CheckCompatibilityWithDefaultSerializator(obj);
			CheckSerializationAndDeserializationByXml(
				@"<TestClassWithOrderParameters type=""ExtendedXmlSerialization.Test.TestObject.TestClassWithOrderParameters"">
  <A>A</A>
  <B>B</B>
</TestClassWithOrderParameters>", obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void XmlElementWithOrderExt()
		{
			var obj = new TestClassWithOrderParametersExt {A = "A", B = "B", C = "C", D = "D"};

			CheckSerializationAndDeserializationByXml(
				@"<TestClassWithOrderParametersExt type=""ExtendedXmlSerialization.Test.TestObject.TestClassWithOrderParametersExt"">
  <A>A</A>
  <B>B</B>
  <D>D</D>
  <C>C</C>
</TestClassWithOrderParametersExt>", obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void TestClassInheritanceWithOrder()
		{
			var obj = new TestClassInheritanceWithOrder();
			obj.Init();
			CheckCompatibilityWithDefaultSerializator(obj);
			CheckSerializationAndDeserializationByXml(
				@"<TestClassInheritanceWithOrder type=""ExtendedXmlSerialization.Test.TestObject.TestClassInheritanceWithOrder"">
    <Id2>3</Id2>  
    <Id>2</Id>
</TestClassInheritanceWithOrder>", obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void TestClassInterfaceInheritanceWithOrder()
		{
			var obj = new TestClassInterfaceInheritanceWithOrder {Id = 1, Id2 = 2};
			CheckCompatibilityWithDefaultSerializator(obj);
			CheckSerializationAndDeserializationByXml(
				@"<TestClassInterfaceInheritanceWithOrder type=""ExtendedXmlSerialization.Test.TestObject.TestClassInterfaceInheritanceWithOrder"">
    <Id2>2</Id2>  
    <Id>1</Id>
</TestClassInterfaceInheritanceWithOrder>", obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void XmlRoot()
		{
			var obj = new TestClassWithXmlRootAttribute {Id = 123};

			CheckSerializationAndDeserializationByXml(
				@"<TestClass type=""ExtendedXmlSerialization.Test.TestObject.TestClassWithXmlRootAttribute"">
  <Id>123</Id>
</TestClass>", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}
	}
}