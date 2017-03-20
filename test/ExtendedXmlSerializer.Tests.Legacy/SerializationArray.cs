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

using System.Collections.ObjectModel;
using ExtendedXmlSerializer.Tests.Legacy.TestObject;
using Xunit;

namespace ExtendedXmlSerializer.Tests.Legacy
{
	// ReSharper disable once TestClassNameSuffixWarning
	public class SerializationArray : BaseTest
	{
		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void ArrayOfObject()
		{
			var obj = new[]
			          {
				          TestClassPrimitiveTypes.Create(),
				          TestClassPrimitiveTypes.Create()
			          };


			CheckSerializationAndDeserialization(
				"ExtendedXmlSerializer.Tests.Legacy.Resources.ArrayOfTestClassPrimitiveTypes.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void ObjectWithArrayProperty()
		{
			var obj = new TestClassWithArray();
			obj.ArrayOfInt = new[] {1, 2, 3};
			CheckSerializationAndDeserialization("ExtendedXmlSerializer.Tests.Legacy.Resources.TestClassWithArray.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void CollectionOfObject()
		{
			var obj = new Collection<TestClassPrimitiveTypes>
			          {
				          TestClassPrimitiveTypes.Create(),
				          TestClassPrimitiveTypes.Create()
			          };

			CheckSerializationAndDeserialization(
				"ExtendedXmlSerializer.Tests.Legacy.Resources.ArrayOfTestClassPrimitiveTypes.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void ClosedGenericCollectionOfObject()
		{
			var obj = new TestClassCollection
			          {
				          TestClassPrimitiveTypes.Create(),
				          TestClassPrimitiveTypes.Create()
			          };

			CheckSerializationAndDeserialization(
				"ExtendedXmlSerializer.Tests.Legacy.Resources.ClosedGenericCollectionOfObject.xml", obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void ClassWithReadOnlyCollectionProperty()
		{
			var obj = new TestClassWithReadOnlyCollectionProperty();
			obj.Items.Add(TestClassPrimitiveTypes.Create());
			obj.Items.Add(TestClassPrimitiveTypes.Create());


			CheckSerializationAndDeserialization(
				"ExtendedXmlSerializer.Tests.Legacy.Resources.ClassWithReadOnlyCollectionProperty.xml",
				obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}
	}
}