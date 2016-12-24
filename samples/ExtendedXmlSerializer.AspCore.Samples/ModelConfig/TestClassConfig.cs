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
using System.Xml.Linq;
using ExtendedXmlSerialization.AspCore.Samples.Model;

namespace ExtendedXmlSerialization.AspCore.Samples.ModelConfig
{
    public class TestClassConfig : ExtendedXmlSerializerConfig<TestClass>
    {
        public TestClassConfig()
        {
            AddMigration(Migration1).AddMigration(Migration2);
        }
        private void Migration1(XElement node)
        {
            var year = node.Element("Year");
            if (year == null) return;

            var newValue = new DateTime(int.Parse(year.Value), 1, 1);
            node.Add(new XElement("DateTime", newValue));
            year.Remove();
        }

        private void Migration2(XElement node)
        {
            node.Add(new XElement("Priority", TestClassPriority.Highest));
        }


    }
}