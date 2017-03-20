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
using System.Collections.Generic;
using ExtendedXmlSerializer.Tests.Legacy.TestObject;
using Xunit;

namespace ExtendedXmlSerializer.Tests.Legacy
{
	// ReSharper disable once TestClassNameSuffixWarning
	public class SerializationObjectProperty : BaseTest
	{
		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void TestClassWithObjectProperty()
		{
			var obj = new List<TestClassWithObjectProperty>
					  {
						  new TestClassWithObjectProperty {TestProperty = 1234},
						  new TestClassWithObjectProperty {TestProperty = "Abc"},
						  new TestClassWithObjectProperty {TestProperty = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 2)}
					  };
			//CheckCompatibilityWithDefaultSerializator(obj);
			CheckSerializationAndDeserializationByXml(
				@"<ArrayOfTestClassWithObjectProperty>
  <TestClassWithObjectProperty type=""ExtendedXmlSerializer.Tests.Legacy.TestObject.TestClassWithObjectProperty"">
	<TestProperty type=""System.Int32"">1234</TestProperty>
  </TestClassWithObjectProperty>
  <TestClassWithObjectProperty type=""ExtendedXmlSerializer.Tests.Legacy.TestObject.TestClassWithObjectProperty"">
	<TestProperty type=""System.String"">Abc</TestProperty>
  </TestClassWithObjectProperty>
  <TestClassWithObjectProperty type=""ExtendedXmlSerializer.Tests.Legacy.TestObject.TestClassWithObjectProperty"">
	<TestProperty type=""System.Guid"">00000001-0002-0003-0405-060708090002</TestProperty>
  </TestClassWithObjectProperty>
</ArrayOfTestClassWithObjectProperty>", obj);
		}
	}
}