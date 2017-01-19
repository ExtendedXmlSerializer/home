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

namespace ExtendedXmlSerialization.Test.TestObject
{
	public class TestClassGeneric<T>
	{
		public T GenericProp { get; set; }
		public string Normal { get; set; }

		public void Init(T genericValue)
		{
			Normal = "normal";
			GenericProp = genericValue;
		}
	}

	public class TestClassGenericThree<T, TK, TL>
	{
		public T GenericProp { get; set; }
		public TK GenericProp2 { get; set; }
		public TL GenericProp3 { get; set; }
		public string Normal { get; set; }

		public void Init(T genericValue, TK genericValue2, TL genericValue3)
		{
			Normal = "normal";
			GenericProp = genericValue;
			GenericProp2 = genericValue2;
			GenericProp3 = genericValue3;
		}
	}

	public class TestClassPropGeneric
	{
		public TestClassGenericThree<string, int, decimal> PropGenric { get; set; }
	}
}