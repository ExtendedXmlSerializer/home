// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using System.Collections.Generic;

namespace ExtendedXmlSerialization.Test.TestObject
{
    public class TestClassFromListWithClass
    {
        public string PropString { get; set; }
        public int PropInt { get; set; }
    }
    public class TestClassWithListWithClass
    {
        public void Init()
        {
            ListStr = new List<string>{"str1", "str2"};
            ListObj = new List<TestClassFromList>{new TestClassFromList{PropString = "s1", PropInt = 1}, 
                                                  new TestClassFromList{PropString = "s2", PropInt = 2}};
            var obj = new TestClassPropIsInterface();
            obj.Init();
            ListWithClass = new List<TestClassPropIsInterface> {obj};
        }
        
        public List<string> ListStr { get; set; }
        public List<TestClassFromList> ListObj { get; set; }
        public List<TestClassPropIsInterface> ListWithClass { get; set; }
    }
}
