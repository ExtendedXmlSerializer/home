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
	public class SerializationStruct : BaseTest
	{
#if !NET451
//TODO Set Single in struct on .net 4.5.1
        [Fact]
        public void Struct()
        {
            var vector2 = new System.Numerics.Vector2(1, 2);

            CheckSerializationAndDeserializationByXml(
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<Vector2 type=""System.Numerics.Vector2""><X>1</X><Y>2</Y></Vector2>",
                vector2);
            CheckCompatibilityWithDefaultSerializator(vector2);
            
        }
#endif

		[Fact]
		public void StructWithConst()
		{
			var obj = new TestStruct();
			obj.Init();
			CheckSerializationAndDeserializationByXml(
				@"<?xml version=""1.0"" encoding=""utf-8""?>
<TestStruct type=""ExtendedXmlSerialization.Test.TestObject.TestStruct""><A>1</A><B>2</B></TestStruct>",
				obj);
			CheckCompatibilityWithDefaultSerializator(obj);
		}
	}
}