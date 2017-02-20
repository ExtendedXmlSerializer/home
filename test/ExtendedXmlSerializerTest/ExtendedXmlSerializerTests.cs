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

// ReSharper disable UnusedAutoPropertyAccessor.Local

using System.Collections.Generic;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.ContentModel.Xml.Namespacing;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
	public class ExtendedXmlSerializerTests
	{
		const string HelloWorld = "Hello World!";
		readonly IExtendedXmlSerializer _serializer = new ExtendedXmlConfiguration().Create();

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
			var instance = new SimpleNestedClass {PropertyName = HelloWorld};
			var data = _serializer.Serialize(instance);
			Assert.Equal(
				@"<?xml version=""1.0"" encoding=""utf-8""?><ExtendedXmlSerializerTests-SimpleNestedClass xmlns=""clr-namespace:ExtendedXmlSerialization.Test;assembly=ExtendedXmlSerializerTest""><PropertyName>Hello World!</PropertyName></ExtendedXmlSerializerTests-SimpleNestedClass>",
				data);
			var read = _serializer.Deserialize<SimpleNestedClass>(data);
			Assert.NotNull(read);
			Assert.Equal(HelloWorld, read.PropertyName);
		}

		[Fact]
		public void ComplexInstance()
		{
			const string expected =
				@"<?xml version=""1.0"" encoding=""utf-8""?><TestClassOtherClass xmlns=""clr-namespace:ExtendedXmlSerialization.Test.TestObject;assembly=ExtendedXmlSerializerTest""><Other><Test><Id>2</Id><Name>Other Name</Name></Test><Double>7.3453145324</Double></Other><Primitive1><PropString>TestString</PropString><PropInt>-1</PropInt><PropuInt>2234</PropuInt><PropDecimal>3.346</PropDecimal><PropDecimalMinValue>-79228162514264337593543950335</PropDecimalMinValue><PropDecimalMaxValue>79228162514264337593543950335</PropDecimalMaxValue><PropFloat>7.4432</PropFloat><PropFloatNaN>NaN</PropFloatNaN><PropFloatPositiveInfinity>INF</PropFloatPositiveInfinity><PropFloatNegativeInfinity>-INF</PropFloatNegativeInfinity><PropFloatMinValue>-3.40282347E+38</PropFloatMinValue><PropFloatMaxValue>3.40282347E+38</PropFloatMaxValue><PropDouble>3.4234</PropDouble><PropDoubleNaN>NaN</PropDoubleNaN><PropDoublePositiveInfinity>INF</PropDoublePositiveInfinity><PropDoubleNegativeInfinity>-INF</PropDoubleNegativeInfinity><PropDoubleMinValue>-1.7976931348623157E+308</PropDoubleMinValue><PropDoubleMaxValue>1.7976931348623157E+308</PropDoubleMaxValue><PropEnum>EnumValue1</PropEnum><PropLong>234234142</PropLong><PropUlong>2345352534</PropUlong><PropShort>23</PropShort><PropUshort>2344</PropUshort><PropDateTime>2014-01-23T00:00:00</PropDateTime><PropByte>23</PropByte><PropSbyte>33</PropSbyte><PropChar>g</PropChar></Primitive1><Primitive2><PropString>TestString</PropString><PropInt>-1</PropInt><PropuInt>2234</PropuInt><PropDecimal>3.346</PropDecimal><PropDecimalMinValue>-79228162514264337593543950335</PropDecimalMinValue><PropDecimalMaxValue>79228162514264337593543950335</PropDecimalMaxValue><PropFloat>7.4432</PropFloat><PropFloatNaN>NaN</PropFloatNaN><PropFloatPositiveInfinity>INF</PropFloatPositiveInfinity><PropFloatNegativeInfinity>-INF</PropFloatNegativeInfinity><PropFloatMinValue>-3.40282347E+38</PropFloatMinValue><PropFloatMaxValue>3.40282347E+38</PropFloatMaxValue><PropDouble>3.4234</PropDouble><PropDoubleNaN>NaN</PropDoubleNaN><PropDoublePositiveInfinity>INF</PropDoublePositiveInfinity><PropDoubleNegativeInfinity>-INF</PropDoubleNegativeInfinity><PropDoubleMinValue>-1.7976931348623157E+308</PropDoubleMinValue><PropDoubleMaxValue>1.7976931348623157E+308</PropDoubleMaxValue><PropEnum>EnumValue1</PropEnum><PropLong>234234142</PropLong><PropUlong>2345352534</PropUlong><PropShort>23</PropShort><PropUshort>2344</PropUshort><PropDateTime>2014-01-23T00:00:00</PropDateTime><PropByte>23</PropByte><PropSbyte>33</PropSbyte><PropChar>g</PropChar></Primitive2><ListProperty><Capacity>32</Capacity><TestClassItem><Id>0</Id><Name>Name 000</Name></TestClassItem><TestClassItem><Id>1</Id><Name>Name 001</Name></TestClassItem><TestClassItem><Id>2</Id><Name>Name 002</Name></TestClassItem><TestClassItem><Id>3</Id><Name>Name 003</Name></TestClassItem><TestClassItem><Id>4</Id><Name>Name 004</Name></TestClassItem><TestClassItem><Id>5</Id><Name>Name 005</Name></TestClassItem><TestClassItem><Id>6</Id><Name>Name 006</Name></TestClassItem><TestClassItem><Id>7</Id><Name>Name 007</Name></TestClassItem><TestClassItem><Id>8</Id><Name>Name 008</Name></TestClassItem><TestClassItem><Id>9</Id><Name>Name 009</Name></TestClassItem><TestClassItem><Id>10</Id><Name>Name 0010</Name></TestClassItem><TestClassItem><Id>11</Id><Name>Name 0011</Name></TestClassItem><TestClassItem><Id>12</Id><Name>Name 0012</Name></TestClassItem><TestClassItem><Id>13</Id><Name>Name 0013</Name></TestClassItem><TestClassItem><Id>14</Id><Name>Name 0014</Name></TestClassItem><TestClassItem><Id>15</Id><Name>Name 0015</Name></TestClassItem><TestClassItem><Id>16</Id><Name>Name 0016</Name></TestClassItem><TestClassItem><Id>17</Id><Name>Name 0017</Name></TestClassItem><TestClassItem><Id>18</Id><Name>Name 0018</Name></TestClassItem><TestClassItem><Id>19</Id><Name>Name 0019</Name></TestClassItem></ListProperty></TestClassOtherClass>";
			var instance = TestClassOtherClass.Create();
			var data = _serializer.Serialize(instance);

			Assert.Equal(expected, data);
			var read = _serializer.Deserialize<TestClassOtherClass>(data);
			Assert.NotNull(read);
			Assert.Equal(expected, _serializer.Serialize(read));
		}

		[Fact]
		public void BasicArray()
		{
			var expected = new[] {1, 2, 3, 4, 5};
			var data = _serializer.Serialize(expected);
			var actual = _serializer.Deserialize<int[]>(data);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void BasicList()
		{
			var expected = new List<string> {"Hello", "World", "Hope", "This", "Works!"};
			var data = _serializer.Serialize(expected);
			var actual = _serializer.Deserialize<List<string>>(data);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ListProperties()
		{
			var expected = new ListWithProperties {"Hello", "World", "Hope", "This", "Works!"};
			expected.Message = HelloWorld;
			expected.Number = 6776;
			var data = _serializer.Serialize(expected);
			var actual = _serializer.Deserialize<ListWithProperties>(data);
			Assert.Equal(expected, actual);
			Assert.Equal(HelloWorld, actual.Message);
			Assert.Equal(6776, actual.Number);
		}

		[Fact]
		public void GenericListProperties()
		{
			var expected = new ListWithProperties<string> {"Hello", "World", "Hope", "This", "Works!"};
			expected.Message = HelloWorld;
			expected.Number = 6776;
			var data = _serializer.Serialize(expected);
			var actual = _serializer.Deserialize<ListWithProperties<string>>(data);
			Assert.Equal(expected, actual);
			Assert.Equal(HelloWorld, actual.Message);
			Assert.Equal(6776, actual.Number);
		}

		[Fact]
		public void BasicHashSet()
		{
			var expected = new HashSet<string> {"Hello", "World", "Hope", "This", "Works!"};
			var data = _serializer.Serialize(expected);
			var actual = _serializer.Deserialize<HashSet<string>>(data);
			Assert.True(actual.SetEquals(expected));
		}

		[Fact]
		public void HashSetProperties()
		{
			var expected = new HashSetWithProperties {"Hello", "World", "Hope", "This", "Works!"};
			expected.Message = HelloWorld;
			expected.Number = 6776;
			var data = _serializer.Serialize(expected);
			var actual = _serializer.Deserialize<HashSetWithProperties>(data);
			Assert.True(actual.SetEquals(expected));
			Assert.Equal(HelloWorld, actual.Message);
			Assert.Equal(6776, actual.Number);
		}

		[Fact]
		public void GenericHashSetWithProperties()
		{
			var expected = new HashSetWithProperties<string> {"Hello", "World", "Hope", "This", "Works!"};
			expected.Message = HelloWorld;
			expected.Number = 6776;
			var data = _serializer.Serialize(expected);
			var actual = _serializer.Deserialize<HashSetWithProperties<string>>(data);
			Assert.True(actual.SetEquals(expected));
			Assert.Equal(HelloWorld, actual.Message);
			Assert.Equal(6776, actual.Number);
		}

		[Fact]
		public void Dictionary()
		{
			var expected = new Dictionary<int, string>{
				               {1, "First"},
				               {2, "Second"},
				               {3, "Other"}
			               };
			var data = _serializer.Serialize(expected);
			var actual = _serializer.Deserialize<Dictionary<int, string>>(data);
			Assert.NotNull(actual);
			Assert.Equal(expected.Count, actual.Count);
			foreach (var entry in actual)
			{
				Assert.Equal(expected[entry.Key], entry.Value);
			}
		}

		[Fact]
		public void DictionaryProperties()
		{
			var expected = new DictionaryWithProperties
			               {
				               {"First", 1},
				               {"Second", 2},
				               {"Other", 3}
			               };
			expected.Message = HelloWorld;
			expected.Number = 6776;

			var data = _serializer.Serialize(expected);
			var actual = _serializer.Deserialize<DictionaryWithProperties>(data);
			Assert.NotNull(actual);
			Assert.Equal(HelloWorld, actual.Message);
			Assert.Equal(6776, actual.Number);
			Assert.Equal(expected.Count, actual.Count);
			foreach (var entry in actual)
			{
				Assert.Equal(expected[entry.Key], entry.Value);
			}
		}

		[Fact]
		public void GenericDictionaryProperties()
		{
			var expected = new GenericDictionaryWithProperties<int, string>
			               {
				               {1, "First"},
				               {2, "Second"},
				               {3, "Other"}
			               };
			expected.Message = HelloWorld;
			expected.Number = 6776;

			var data = _serializer.Serialize(expected);
			var actual = _serializer.Deserialize<GenericDictionaryWithProperties<int, string>>(data);
			Assert.NotNull(actual);
			Assert.Equal(HelloWorld, actual.Message);
			Assert.Equal(6776, actual.Number);
			Assert.Equal(expected.Count, actual.Count);
			foreach (var entry in actual)
			{
				Assert.Equal(expected[entry.Key], entry.Value);
			}
		}

		public class ListWithProperties : List<string>
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}

		public class ListWithProperties<T> : List<T>
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}

		public class HashSetWithProperties<T> : HashSet<T>
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}

		public class HashSetWithProperties : HashSet<string>
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}

		public class GenericDictionaryWithProperties<TKey, TValue> : Dictionary<TKey, TValue>
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}

		public class DictionaryWithProperties : Dictionary<string, int>
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}

		[Fact]
		public void Generic()
		{
			var expected = new TestClassGenericThree<string, int, TestClassPrimitiveTypes>();
			expected.Init("StringValue", 1, TestClassPrimitiveTypes.Create());

			var data = _serializer.Serialize(expected);
			var actual = _serializer.Deserialize<TestClassGenericThree<string, int, TestClassPrimitiveTypes>>(data);
			Assert.NotNull(actual);
			Assert.Equal(expected.GenericProp, actual.GenericProp);
			Assert.Equal(expected.GenericProp2, actual.GenericProp2);
			Assert.Equal(expected.GenericProp3.PropDateTime, actual.GenericProp3.PropDateTime);
			Assert.Equal(data, _serializer.Serialize(actual));
		}

		[Fact]
		public void DifferingProperty()
		{
			const string message = "Hello World!  This is a value set in a property with a variable type.";
			var expected = new ClassWithDifferingPropertyType { Interface = new Implementation { PropertyName = message } };
			var data = _serializer.Serialize(expected);
			var actual = _serializer.Deserialize<ClassWithDifferingPropertyType>(data);
			var implementation = Assert.IsType<Implementation>(actual.Interface);
			Assert.Equal(message, implementation.PropertyName);
		}

		[Fact]
		public void Optimized()
		{
			const string message = "Hello World!  This is a value set in a property with a variable type.", 
				expected = @"<?xml version=""1.0"" encoding=""utf-8""?><ExtendedXmlSerializerTests-ClassWithDifferingPropertyType xmlns=""clr-namespace:ExtendedXmlSerialization.Test;assembly=ExtendedXmlSerializerTest"" xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2""><Interface exs:type=""ExtendedXmlSerializerTests-Implementation""><PropertyName>Hello World!  This is a value set in a property with a variable type.</PropertyName></Interface></ExtendedXmlSerializerTests-ClassWithDifferingPropertyType>";
			var instance = new ClassWithDifferingPropertyType { Interface = new Implementation { PropertyName = message } };
			var serializer = new ExtendedXmlConfiguration(Factory.Default).Create();
			var data = serializer.Serialize(instance);
			Assert.Equal(expected, data);
		}

		class Factory : IExtendedXmlSerializerFactory
		{
			public static Factory Default { get; } = new Factory();
			Factory() {}

			public IExtendedXmlSerializer Get(IExtendedXmlConfiguration parameter)
			{
				var serializers = SerializationFactory.Default.Get(parameter);
				var result = new ExtendedXmlSerializer(TypeSelector.Default, new OptimizedXmlFactory(serializers), serializers);
				return result;
			}
		}

#if CORE
		[Fact]
		public void OptimizedList()
		{
			const string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><ExtendedXmlSerializerTests-ClassWithDifferingPropertyType xmlns=""clr-namespace:ExtendedXmlSerialization.Test;assembly=ExtendedXmlSerializerTest"" xmlns:ns1=""clr-namespace:System.Collections.Generic;assembly=System.Collections"" xmlns:sys=""https://github.com/wojtpl2/ExtendedXmlSerializer/system"" xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2""><Interface exs:type=""ExtendedXmlSerializerTests-GeneralImplementation""><Instance exs:type=""ns1:HashSet[sys:string]""><sys:string>Hello</sys:string><sys:string>World</sys:string><sys:string>Hope</sys:string><sys:string>This</sys:string><sys:string>Works!</sys:string></Instance></Interface></ExtendedXmlSerializerTests-ClassWithDifferingPropertyType>";
			var instance = new ClassWithDifferingPropertyType { Interface = new GeneralImplementation { Instance = new HashSet<string> {"Hello", "World", "Hope", "This", "Works!"} } };
			var serializer = new ExtendedXmlConfiguration(Factory.Default).Create();
			var data = serializer.Serialize(instance);
			Assert.Equal(expected, data);
		}
#else
		[Fact]
		public void OptimizedList()
		{
			const string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><ExtendedXmlSerializerTests-ClassWithDifferingPropertyType xmlns=""clr-namespace:ExtendedXmlSerialization.Test;assembly=ExtendedXmlSerializerTest"" xmlns:ns1=""clr-namespace:System.Collections.Generic;assembly=System.Core"" xmlns:sys=""https://github.com/wojtpl2/ExtendedXmlSerializer/system"" xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2""><Interface exs:type=""ExtendedXmlSerializerTests-GeneralImplementation""><Instance exs:type=""ns1:HashSet[sys:string]""><sys:string>Hello</sys:string><sys:string>World</sys:string><sys:string>Hope</sys:string><sys:string>This</sys:string><sys:string>Works!</sys:string></Instance></Interface></ExtendedXmlSerializerTests-ClassWithDifferingPropertyType>";
			var instance = new ClassWithDifferingPropertyType { Interface = new GeneralImplementation { Instance = new HashSet<string> {"Hello", "World", "Hope", "This", "Works!"} } };
			var serializer = new ExtendedXmlConfiguration(Factory.Default).Create();
			var data = serializer.Serialize(instance);
			Assert.Equal(expected, data);
		}
#endif
		class SimpleNestedClass
		{
			public string PropertyName { get; set; }
		}

		class ClassWithDifferingPropertyType
		{
			public IInterface Interface { get; set; }
		}

		public interface IInterface {}

		class Implementation : IInterface
		{
			public string PropertyName { get; set; }
		}

		class GeneralImplementation : IInterface
		{
			public object Instance { get; set; }
		}
	}
}