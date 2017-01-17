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
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class SerializationDictionary : BaseTest
    {
        public class TestClass
        {
            public Dictionary<int, string> Dictionary { get; set; }
        }

        [Fact]
        public void DictionaryOfIntAndString()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "First");
            dict.Add(2, "Second");
            dict.Add(3, "Other");

            CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<ArrayOfInt32String>
    <Item>
        <Key>1</Key>
        <Value>First</Value>
    </Item>
    <Item>
        <Key>2</Key>
        <Value>Second</Value>
    </Item>
    <Item>
        <Key>3</Key>
        <Value>Other</Value>
    </Item>
</ArrayOfInt32String>", dict);
        }

        [Fact]
        public void PropertyDictionary()
        {
            var obj = new TestClass
                      {
                          Dictionary = new Dictionary<int, string>
                                       {
                                           {1, "First"},
                                           {2, "Second"},
                                           {3, "Other"},
                                       }
                      };

            CheckSerializationAndDeserializationByXml(@"<TestClass type=""ExtendedXmlSerialization.Test.SerializationDictionary+TestClass"">
  <Dictionary>
    <Item>
        <Key>1</Key>
        <Value>First</Value>
    </Item>
    <Item>
        <Key>2</Key>
        <Value>Second</Value>
    </Item>
    <Item>
        <Key>3</Key>
        <Value>Other</Value>
    </Item>
  </Dictionary>
</TestClass>", obj);
        }
    }
}