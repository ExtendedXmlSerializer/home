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

namespace ExtendedXmlSerialization.Samples.MigrationMap
{
    /*
// FirstVersion
    public class TestClass
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
// EndFirstVersion

// XmlFirstVersion
<? xml version="1.0" encoding="utf-8"?>
<TestClass xmlns="clr-namespace:ExtendedXmlSerialization.Samples.MigrationMap;assembly=ExtendedXmlSerializer.Samples">
  <Id>1</Id>
  <Type>Type</Type>
</TestClass>
// EndXmlFirstVersion

// SecondVersion
    public class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
// EndSecondVersion

// XmlSecondVersion
<? xml version="1.0" encoding="utf-8"?>
<TestClass xmlns:exs="https://extendedxmlserializer.github.io/v2" exs:version="1" xmlns="clr-namespace:ExtendedXmlSerialization.Samples.MigrationMap;assembly=ExtendedXmlSerializer.Samples">
  <Id>1</Id>
  <Name>Type</Name>
</TestClass>
// EndXmlSecondVersion
*/

    // LastVersion
    public class TestClass
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
	}
// EndLastVersion
}