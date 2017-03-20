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

using ExtendedXmlSerializer.Tests.Legacy.TestObject;
using Xunit;

namespace ExtendedXmlSerializer.Tests.Legacy
{
	// ReSharper disable once TestClassNameSuffixWarning
	public class SerializationGeneric : BaseTest
	{
		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void TestClassGeneric()
		{
			var obj = new TestClassGeneric<string>();
			obj.Init("StringValue");

			CheckSerializationAndDeserialization("ExtendedXmlSerializer.Tests.Legacy.Resources.TestClassGeneric.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void TestClassGenericThree()
		{
			var obj = new TestClassGenericThree<string, int, TestClassPrimitiveTypes>();
			obj.Init("StringValue", 1, TestClassPrimitiveTypes.Create());
			
			CheckSerializationAndDeserialization("ExtendedXmlSerializer.Tests.Legacy.Resources.TestClassGenericThree.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void TestClassPropGeneric()
		{
			var pop = new TestClassGenericThree<string, int, decimal>();
			pop.Init("StringValue", 1, 4.4m);
			var obj = new TestClassPropGeneric();
			obj.PropGenric = pop;


			CheckSerializationAndDeserialization("ExtendedXmlSerializer.Tests.Legacy.Resources.TestClassPropGeneric.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}
	}
}