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
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.Samples.CustomSerializator
{
// TestClassSerializer
    public class TestClassSerializer : IExtendedXmlCustomSerializer<TestClass>
    {
        public TestClass Deserialize(XElement element)
        {
            var xElement = element.Member("String");
            var xElement1 = element.Member("Int");
            if (xElement != null && xElement1 != null)
            {
                var strValue = xElement.Value;

                var intValue = Convert.ToInt32(xElement1.Value);
                return new TestClass(strValue, intValue);
            }
            throw new InvalidOperationException("Invalid xml for class TestClassWithSerializer");
        }

        public void Serializer(XmlWriter writer, TestClass obj)
        {
            writer.WriteElementString("String", obj.PropStr);
            writer.WriteElementString("Int", obj.PropInt.ToString(CultureInfo.InvariantCulture));
        }
    }
// EndTestClassSerializer
}