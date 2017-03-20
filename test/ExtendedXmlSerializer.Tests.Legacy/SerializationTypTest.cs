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
	public class SerializationTypTest : BaseTest
	{
		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void ClassPrimitiveTypes()
		{
			var obj = TestClassPrimitiveTypes.Create();
			
			CheckSerializationAndDeserialization("ExtendedXmlSerializer.Tests.Legacy.Resources.TestClassPrimitiveTypes.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void ClassPrimitiveTypesNullable()
		{
			var obj = new TestClassPrimitiveTypesNullable();
			obj.Init();

			CheckSerializationAndDeserialization(
				"ExtendedXmlSerializer.Tests.Legacy.Resources.TestClassPrimitiveTypesNullable.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void ClassPrimitiveTypesNullableSetNull()
		{
			var obj = new TestClassPrimitiveTypesNullable();
			obj.InitNull();

			CheckSerializationAndDeserialization(
				"ExtendedXmlSerializer.Tests.Legacy.Resources.TestClassPrimitiveTypesNullableSetNull.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void ClassWithList()
		{
			var obj = new TestClassWithList();
			obj.Init();

			CheckSerializationAndDeserialization("ExtendedXmlSerializer.Tests.Legacy.Resources.TestClassWithList.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void ClassWithHashSet()
		{
			var obj = new TestClassWithHashSet();
			obj.Init();

			CheckSerializationAndDeserialization("ExtendedXmlSerializer.Tests.Legacy.Resources.TestClassWithHashSet.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void TestClassWithListWithClass()
		{
			var obj = new TestClassWithListWithClass();
			obj.Init();

			CheckSerializationAndDeserialization("ExtendedXmlSerializer.Tests.Legacy.Resources.TestClassWithListWithClass.xml",
			                                     obj);
//            CheckCompatibilityWithDefaultSerializator(obj);
		}


		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void TestClassPropIsInterface()
		{
			var obj = new TestClassPropIsInterface();
			obj.Init();

			CheckSerializationAndDeserialization("ExtendedXmlSerializer.Tests.Legacy.Resources.TestClassPropIsInterface.xml", obj);
			//CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void TestClassGuid()
		{
			var obj = new TestClassGuid();
			obj.Init();

			CheckSerializationAndDeserialization("ExtendedXmlSerializer.Tests.Legacy.Resources.TestClassGuid.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void TestClassTimeSpan()
		{
			var obj = new TestClassTimeSpan();
			obj.Init();

			CheckSerializationAndDeserialization("ExtendedXmlSerializer.Tests.Legacy.Resources.TestClassTimeSpan.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}
	}
}