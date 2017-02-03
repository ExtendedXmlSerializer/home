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

using System.Collections.Generic;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
	public class ExtendedXmlSerializerTests
	{
		readonly TestClassOtherClass _subject = TestClassOtherClass.Create();

		readonly IExtendedXmlSerializer _serializer = /*new ExtendedXmlConfiguration().Create()*/new ExtendedXmlSerializer();

		public ExtendedXmlSerializerTests()
		{
			_serializer.Serialize(_subject);
		}

		[Fact]
		public void Primitive()
		{
			const int expected = 6776;

			var data = _serializer.Serialize(expected);
			Assert.Equal(
				@"<?xml version=""1.0"" encoding=""utf-8""?><int xmlns=""https://github.com/wojtpl2/ExtendedXmlSerializer/system"">6776</int>",
				data);
			var actual = _serializer.Deserialize(data);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void SimpleInstance()
		{
			var instance = new InstanceClass {PropertyName = "Hello World!"};
			var data = _serializer.Serialize(instance);
			Assert.Equal(
				@"<?xml version=""1.0"" encoding=""utf-8""?><ExtendedXmlSerializerTests-InstanceClass xmlns=""clr-namespace:ExtendedXmlSerialization.Test;assembly=ExtendedXmlSerializerTest""><PropertyName>Hello World!</PropertyName></ExtendedXmlSerializerTests-InstanceClass>",
				data);
			var read = _serializer.Deserialize<InstanceClass>(data);
			Assert.NotNull(read);
			Assert.Equal("Hello World!", read.PropertyName);
		}

/*
		[Fact]
		public void ProfileLoop()
		{
			for (int i = 0; i < 100; i++)
			{
				_serializer.Serialize(_subject);
			}
		}
*/

		[Fact]
		public void ComplexInstance()
		{
			const string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassOtherClass xmlns=""clr-namespace:ExtendedXmlSerialization.Test;assembly=ExtendedXmlSerializerTest""><Other><Test><Id>2</Id><Name>Other Name</Name></Test><Double>7.3453145324</Double></Other><Primitive1><PropString>TestString</PropString><PropInt>-1</PropInt><PropuInt>2234</PropuInt><PropDecimal>3.346</PropDecimal><PropDecimalMinValue>-79228162514264337593543950335</PropDecimalMinValue><PropDecimalMaxValue>79228162514264337593543950335</PropDecimalMaxValue><PropFloat>7.4432</PropFloat><PropFloatNaN>NaN</PropFloatNaN><PropFloatPositiveInfinity>INF</PropFloatPositiveInfinity><PropFloatNegativeInfinity>-INF</PropFloatNegativeInfinity><PropFloatMinValue>-3.40282347E+38</PropFloatMinValue><PropFloatMaxValue>3.40282347E+38</PropFloatMaxValue><PropDouble>3.4234</PropDouble><PropDoubleNaN>NaN</PropDoubleNaN><PropDoublePositiveInfinity>INF</PropDoublePositiveInfinity><PropDoubleNegativeInfinity>-INF</PropDoubleNegativeInfinity><PropDoubleMinValue>-1.7976931348623157E+308</PropDoubleMinValue><PropDoubleMaxValue>1.7976931348623157E+308</PropDoubleMaxValue><PropEnum>EnumValue1</PropEnum><PropLong>234234142</PropLong><PropUlong>2345352534</PropUlong><PropShort>23</PropShort><PropUshort>2344</PropUshort><PropDateTime>2014-01-23T00:00:00</PropDateTime><PropByte>23</PropByte><PropSbyte>33</PropSbyte><PropChar>g</PropChar></Primitive1><Primitive2><PropString>TestString</PropString><PropInt>-1</PropInt><PropuInt>2234</PropuInt><PropDecimal>3.346</PropDecimal><PropDecimalMinValue>-79228162514264337593543950335</PropDecimalMinValue><PropDecimalMaxValue>79228162514264337593543950335</PropDecimalMaxValue><PropFloat>7.4432</PropFloat><PropFloatNaN>NaN</PropFloatNaN><PropFloatPositiveInfinity>INF</PropFloatPositiveInfinity><PropFloatNegativeInfinity>-INF</PropFloatNegativeInfinity><PropFloatMinValue>-3.40282347E+38</PropFloatMinValue><PropFloatMaxValue>3.40282347E+38</PropFloatMaxValue><PropDouble>3.4234</PropDouble><PropDoubleNaN>NaN</PropDoubleNaN><PropDoublePositiveInfinity>INF</PropDoublePositiveInfinity><PropDoubleNegativeInfinity>-INF</PropDoubleNegativeInfinity><PropDoubleMinValue>-1.7976931348623157E+308</PropDoubleMinValue><PropDoubleMaxValue>1.7976931348623157E+308</PropDoubleMaxValue><PropEnum>EnumValue1</PropEnum><PropLong>234234142</PropLong><PropUlong>2345352534</PropUlong><PropShort>23</PropShort><PropUshort>2344</PropUshort><PropDateTime>2014-01-23T00:00:00</PropDateTime><PropByte>23</PropByte><PropSbyte>33</PropSbyte><PropChar>g</PropChar></Primitive2><ListProperty><TestClassItem><Id>0</Id><Name>Name 000</Name></TestClassItem><TestClassItem><Id>1</Id><Name>Name 001</Name></TestClassItem><TestClassItem><Id>2</Id><Name>Name 002</Name></TestClassItem><TestClassItem><Id>3</Id><Name>Name 003</Name></TestClassItem><TestClassItem><Id>4</Id><Name>Name 004</Name></TestClassItem><TestClassItem><Id>5</Id><Name>Name 005</Name></TestClassItem><TestClassItem><Id>6</Id><Name>Name 006</Name></TestClassItem><TestClassItem><Id>7</Id><Name>Name 007</Name></TestClassItem><TestClassItem><Id>8</Id><Name>Name 008</Name></TestClassItem><TestClassItem><Id>9</Id><Name>Name 009</Name></TestClassItem><TestClassItem><Id>10</Id><Name>Name 0010</Name></TestClassItem><TestClassItem><Id>11</Id><Name>Name 0011</Name></TestClassItem><TestClassItem><Id>12</Id><Name>Name 0012</Name></TestClassItem><TestClassItem><Id>13</Id><Name>Name 0013</Name></TestClassItem><TestClassItem><Id>14</Id><Name>Name 0014</Name></TestClassItem><TestClassItem><Id>15</Id><Name>Name 0015</Name></TestClassItem><TestClassItem><Id>16</Id><Name>Name 0016</Name></TestClassItem><TestClassItem><Id>17</Id><Name>Name 0017</Name></TestClassItem><TestClassItem><Id>18</Id><Name>Name 0018</Name></TestClassItem><TestClassItem><Id>19</Id><Name>Name 0019</Name></TestClassItem></ListProperty></TestClassOtherClass>";
			var instance = TestClassOtherClass.Create();
			var data = _serializer.Serialize(instance);
			
			Assert.Equal(expected, data);
			var read = _serializer.Deserialize<TestClassOtherClass>(data);
			Assert.NotNull(read);
			Assert.Equal(expected, _serializer.Serialize(read));
		}

/*
		[Fact]
		public void Dictionary()
		{
			/*var instance = new Dictionary<int, string>
			               {
				               {1, "First"},
				               {2, "Second"},
				               {3, "Other"}
			               };
			var data = _serializer.Serialize(instance);
			Assert.Equal(
				@"<?xml version=""1.0"" encoding=""utf-8""?><DictionaryOfInt32String xmlns=""https://github.com/wojtpl2/ExtendedXmlSerializer/system""><Item><Key>1</Key><Value>First</Value></Item><Item><Key>2</Key><Value>Second</Value></Item><Item><Key>3</Key><Value>Other</Value></Item></DictionaryOfInt32String>",
				data);#1#
			/*var read = ExtendedXmlSerializer.Default.Deserialize<Dictionary<int, string>>(data);
			Assert.NotNull(read);
			Assert.Equal(instance.Count, read.Count);
			foreach (var entry in read)
			{
				Assert.Equal(instance[entry.Key], entry.Value);
			}#1#
		}
*/

		class InstanceClass
		{
			public string PropertyName { get; set; }
		}
	}

	public class TestClassOtherClass
	{
		public static TestClassOtherClass Create()
		{
			var result = new TestClassOtherClass
			             {
				             Primitive1 = TestClassPrimitiveTypes.Create(),
				             Primitive2 = TestClassPrimitiveTypes.Create(),
				             ListProperty = new List<TestClassItem>(),
				             Other = new TestClassOther()
			             };
			for (var i = 0; i < 20; i++)
			{
				result.ListProperty.Add(new TestClassItem {Id = i, Name = $"Name 00{i.ToString()}"});
			}

			return result;
		}

		public TestClassOther Other { get; set; }
		public TestClassPrimitiveTypes Primitive1 { get; set; }
		public TestClassPrimitiveTypes Primitive2 { get; set; }

		public List<TestClassItem> ListProperty { get; set; }
	}

	public class TestClassItem
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class TestClassOther
	{
		public TestClassOther()
		{
			Test = new TestClassItem {Id = 2, Name = "Other Name"};
			Double = 7.3453145324;
		}

		public TestClassItem Test { get; set; }

		public double Double { get; set; }
	}
}