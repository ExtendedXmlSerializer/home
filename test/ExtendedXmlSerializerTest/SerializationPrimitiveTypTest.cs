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
using System;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class SerializationPrimitiveTypTest: BaseTest
    {
        [Fact]
        public void SerializeString()
        {
            var obj = "test";

            CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?><string>test</string>", obj);
            CheckCompatibilityWithDefaultSerializator(obj);
        }

        [Fact]
        public void SerializeBool()
        {
            var obj = true;

            CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<boolean>True</boolean>", obj);
            CheckCompatibilityWithDefaultSerializator(obj);
        }

        [Fact]
        public void SerializeInt()
        {
            var obj = 456;

            CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<int>456</int>", obj);
            CheckCompatibilityWithDefaultSerializator(obj);
        }

        [Fact]
        public void SerializeDecimal()
        {
            var obj = 8.4m;

            CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<decimal>8.4</decimal>", obj);
            CheckCompatibilityWithDefaultSerializator(obj);
        }

        [Fact]
        public void SerializeArrayOfInt()
        {
            var obj = new int[] {1, 2, 3};

            CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<ArrayOfInt32>
  <int>1</int>
  <int>2</int>
  <int>3</int>
</ArrayOfInt32>", obj);
            CheckCompatibilityWithDefaultSerializator(obj);
        }

        [Fact]
        public void SerializeList()
        {
            var obj = new List<int>() {1, 2, 3};

            CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<ArrayOfInt32>
  <int>1</int>
  <int>2</int>
  <int>3</int>
</ArrayOfInt32>", obj);
            CheckCompatibilityWithDefaultSerializator(obj);
        }

        [Fact]
        public void SerializeHashSet()
        {
            var obj = new HashSet<int>() { 1, 2, 3 };

            CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<ArrayOfInt32>
  <int>1</int>
  <int>2</int>
  <int>3</int>
</ArrayOfInt32>", obj);
            CheckCompatibilityWithDefaultSerializator(obj);
        }

        [Fact]
        public void SerializeGuid()
        {
            var obj = new Guid("7db85a35-1f66-4e5c-9c4a-33a937a9258b");
            CheckCompatibilityWithDefaultSerializator(obj);
            CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?><guid>7db85a35-1f66-4e5c-9c4a-33a937a9258b</guid>", obj);
            
        }

        [Fact]
        public void SerializeTimeStamp()
        {
            var obj = new TimeSpan(1, 2, 3, 4);
#if !NET451
            //Serialization TimeSpan on .net 4.5.1 not work
            CheckCompatibilityWithDefaultSerializator(obj);
#endif
            CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?><TimeSpan>P1DT2H3M4S</TimeSpan>", obj);

        }

        [Fact]
        public void SerializeNullableTyp()
        {
            int? obj = 1;
            CheckCompatibilityWithDefaultSerializator(obj);
            CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?><int>1</int>", obj);

        }
    }
}
