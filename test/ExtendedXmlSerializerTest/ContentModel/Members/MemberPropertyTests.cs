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
using System.Reflection;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.ContentModel.Members;
using Xunit;

namespace ExtendedXmlSerialization.Test.ContentModel.Members
{
	public class MemberPropertyTests
	{
		const string HelloWorld = "Hello World!";
		readonly IExtendedXmlSerializer _serializer = new ExtendedXmlConfiguration().Create();

		[Fact]
		public void BasicAttributes()
		{
			var converters = new Dictionary<MemberInfo, IConverter>
			                 {
				                 {typeof(SimpleSubject).GetRuntimeProperty(nameof(SimpleSubject.Message)), StringConverter.Default},
				                 {typeof(SimpleSubject).GetRuntimeProperty(nameof(SimpleSubject.Number)), IntegerConverter.Default}
			                 };
			var configuration = new ExtendedXmlConfiguration(converters,
			                                                 new Dictionary<MemberInfo, IMemberEmitSpecification>(),
			                                                 new Dictionary<MemberInfo, IRuntimeMemberSpecification>());

			var serializer = configuration.Create();

			var expected = new SimpleSubject {Message = "Hello World!", Number = 6776};
			var data = serializer.Serialize(expected);
			var actual = serializer.Deserialize<SimpleSubject>(data);
			Assert.Equal(expected.Message, actual.Message);
			Assert.Equal(expected.Number, actual.Number);
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
		public void ListAttributes()
		{
			var converters = new Dictionary<MemberInfo, IConverter>
			                 {
				                 {
					                 typeof(ListWithProperties).GetRuntimeProperty(nameof(ListWithProperties.Message)),
					                 StringConverter.Default
				                 },
				                 {
					                 typeof(ListWithProperties).GetRuntimeProperty(nameof(ListWithProperties.Number)),
					                 IntegerConverter.Default
				                 }
			                 };
			var configuration = new ExtendedXmlConfiguration(converters,
			                                                 new Dictionary<MemberInfo, IMemberEmitSpecification>(),
			                                                 new Dictionary<MemberInfo, IRuntimeMemberSpecification>());

			var serializer = configuration.Create();

			var expected = new ListWithProperties {"Hello", "World", "Hope", "This", "Works!"};
			expected.Message = HelloWorld;
			expected.Number = 6776;
			var data = serializer.Serialize(expected);
			var actual = serializer.Deserialize<ListWithProperties>(data);
			Assert.Equal(expected.Message, actual.Message);
			Assert.Equal(expected.Number, actual.Number);
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
		public void GenericListAttributes()
		{
			var converters = new Dictionary<MemberInfo, IConverter>
			                 {
				                 //{typeof(ListWithProperties<string>).GetRuntimeProperty(nameof(Subject.Message)), StringConverter.Default},
				                 {
					                 typeof(ListWithProperties<string>).GetRuntimeProperty(nameof(SimpleSubject.Number)),
					                 IntegerConverter.Default
				                 }
			                 };
			var configuration = new ExtendedXmlConfiguration(converters,
			                                                 new Dictionary<MemberInfo, IMemberEmitSpecification>(),
			                                                 new Dictionary<MemberInfo, IRuntimeMemberSpecification>());

			var serializer = configuration.Create();

			var expected = new ListWithProperties<string> {"Hello", "World", "Hope", "This", "Works!"};
			expected.Message = HelloWorld;
			expected.Number = 6776;
			var data = serializer.Serialize(expected);
			var actual = serializer.Deserialize<ListWithProperties<string>>(data);
			Assert.Equal(expected, actual);
			Assert.Equal(expected.Message, actual.Message);
			Assert.Equal(expected.Number, actual.Number);
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
		public void HashSetAttributes()
		{
			var converters = new Dictionary<MemberInfo, IConverter>
			                 {
				                 //{typeof(ListWithProperties<string>).GetRuntimeProperty(nameof(Subject.Message)), StringConverter.Default},
				                 {
					                 typeof(HashSetWithProperties).GetRuntimeProperty(nameof(SimpleSubject.Number)),
					                 IntegerConverter.Default
				                 }
			                 };
			var configuration = new ExtendedXmlConfiguration(converters,
			                                                 new Dictionary<MemberInfo, IMemberEmitSpecification>(),
			                                                 new Dictionary<MemberInfo, IRuntimeMemberSpecification>());

			var serializer = configuration.Create();

			var expected = new HashSetWithProperties {"Hello", "World", "Hope", "This", "Works!"};
			expected.Message = HelloWorld;
			expected.Number = 6776;
			var data = serializer.Serialize(expected);
			var actual = serializer.Deserialize<HashSetWithProperties>(data);
			Assert.True(actual.SetEquals(expected));
			Assert.Equal(expected.Message, actual.Message);
			Assert.Equal(expected.Number, actual.Number);
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
		public void GenericHashSetAttributes()
		{
			var converters = new Dictionary<MemberInfo, IConverter>
			                 {
				                 //{typeof(ListWithProperties<string>).GetRuntimeProperty(nameof(Subject.Message)), StringConverter.Default},
				                 {
					                 typeof(HashSetWithProperties<string>).GetRuntimeProperty(nameof(SimpleSubject.Number)),
					                 IntegerConverter.Default
				                 }
			                 };
			var configuration = new ExtendedXmlConfiguration(converters,
			                                                 new Dictionary<MemberInfo, IMemberEmitSpecification>(),
			                                                 new Dictionary<MemberInfo, IRuntimeMemberSpecification>());

			var serializer = configuration.Create();

			var expected = new HashSetWithProperties<string> {"Hello", "World", "Hope", "This", "Works!"};
			expected.Message = HelloWorld;
			expected.Number = 6776;
			var data = serializer.Serialize(expected);
			var actual = serializer.Deserialize<HashSetWithProperties<string>>(data);
			Assert.True(actual.SetEquals(expected));
			Assert.Equal(expected.Message, actual.Message);
			Assert.Equal(expected.Number, actual.Number);
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
		public void DictionaryAttributes()
		{
			var converters = new Dictionary<MemberInfo, IConverter>
			                 {
				                 //{typeof(ListWithProperties<string>).GetRuntimeProperty(nameof(Subject.Message)), StringConverter.Default},
				                 {
					                 typeof(DictionaryWithProperties).GetRuntimeProperty(nameof(SimpleSubject.Number)),
					                 IntegerConverter.Default
				                 }
			                 };
			var configuration = new ExtendedXmlConfiguration(converters,
			                                                 new Dictionary<MemberInfo, IMemberEmitSpecification>(),
			                                                 new Dictionary<MemberInfo, IRuntimeMemberSpecification>());

			var serializer = configuration.Create();

			var expected = new DictionaryWithProperties
			               {
				               {"First", 1},
				               {"Second", 2},
				               {"Other", 3}
			               };
			expected.Message = HelloWorld;
			expected.Number = 6776;

			var data = serializer.Serialize(expected);
			var actual = serializer.Deserialize<DictionaryWithProperties>(data);
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

		[Fact]
		public void GenericDictionaryAttributes()
		{
			var converters = new Dictionary<MemberInfo, IConverter>
			                 {
				                 {
					                 typeof(GenericDictionaryWithProperties<int, string>).GetRuntimeProperty(
						                 nameof(SimpleSubject.Message)),
					                 StringConverter.Default
				                 },
				                 {
					                 typeof(GenericDictionaryWithProperties<int, string>).GetRuntimeProperty(
						                 nameof(SimpleSubject.Number)),
					                 IntegerConverter.Default
				                 }
			                 };
			var configuration = new ExtendedXmlConfiguration(converters,
			                                                 new Dictionary<MemberInfo, IMemberEmitSpecification>(),
			                                                 new Dictionary<MemberInfo, IRuntimeMemberSpecification>());

			var serializer = configuration.Create();

			var expected = new GenericDictionaryWithProperties<int, string>
			               {
				               {1, "First"},
				               {2, "Second"},
				               {3, "Other"}
			               };
			expected.Message = HelloWorld;
			expected.Number = 6776;

			var data = serializer.Serialize(expected);
			var actual = serializer.Deserialize<GenericDictionaryWithProperties<int, string>>(data);
			Assert.NotNull(actual);
			Assert.Equal(HelloWorld, actual.Message);
			Assert.Equal(6776, actual.Number);
			Assert.Equal(expected.Count, actual.Count);
			foreach (var entry in actual)
			{
				Assert.Equal(expected[entry.Key], entry.Value);
			}
		}

		class ListWithProperties : List<string>
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}

		class ListWithProperties<T> : List<T>
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}

		class HashSetWithProperties<T> : HashSet<T>
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}

		class HashSetWithProperties : HashSet<string>
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}

		class GenericDictionaryWithProperties<TKey, TValue> : Dictionary<TKey, TValue>
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}

		class DictionaryWithProperties : Dictionary<string, int>
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}

		class SimpleSubject
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}
	}
}