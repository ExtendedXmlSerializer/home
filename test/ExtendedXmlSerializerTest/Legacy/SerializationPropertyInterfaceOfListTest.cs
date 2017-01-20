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
	public class SerializationPropertyInterfaceOfListTest : BaseTest
	{
		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void PropertyInterfaceOfList()
		{
			var obj = new TestClassPropertyInterfaceOfList();
			obj.Init();

			CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<TestClassPropertyInterfaceOfList type=""ExtendedXmlSerialization.Test.TestObject.TestClassPropertyInterfaceOfList"">
<List type=""System.Collections.Generic.List`1[[System.String, [CORELIB]]]""><string>Item1</string></List>
<Dictionary type=""System.Collections.Generic.Dictionary`2[[System.String, [CORELIB]],[System.String, [CORELIB]]]""><Item><Key>Key</Key><Value>Value</Value></Item></Dictionary>
<Set type=""System.Collections.Generic.HashSet`1[[System.String, [CORELIB]]]""><string>Item1</string></Set>
</TestClassPropertyInterfaceOfList>", obj);
		}
	}
}