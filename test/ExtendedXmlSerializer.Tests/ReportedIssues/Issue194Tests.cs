using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue194Tests
	{
		interface IDescribable
		{
			string Description { get; set; }
		}

		// ReSharper disable once UnusedTypeParameter
		class ClassWithGeneric<T> : IDescribable
		{
			public string Description { get; set; }
		}

		[Fact]
		void GenericSimpleValue()
		{
			var serializer = new ConfigurationContainer()
			                 .Create()
			                 .ForTesting();

			var singleValue = new ClassWithGeneric<int>();
			singleValue.Description = "testtest123";

			var result = serializer.Serialize(singleValue);

			var deserializedValue = serializer.Deserialize<ClassWithGeneric<int>>(result);

			deserializedValue.Description.Should()
			                 .Be("testtest123");
		}

		[Fact]
		void GenericListValues()
		{
			var serializer = new ConfigurationContainer()
			                 .Create()
			                 .ForTesting();

			var singleValue = new ClassWithGeneric<int>();
			singleValue.Description = "testtest345";

			var list = new List<IDescribable>()
			{
				singleValue
			};

			var result = serializer.Serialize(list);

			var deserializedValue = serializer.Deserialize<List<IDescribable>>(result);

			deserializedValue.First()
			                 .Description.Should()
			                 .Be("testtest345");
		}

		[Fact]
		void GenericLinkedListValues()
		{
			var serializer = new ConfigurationContainer()
			                 .Create()
			                 .ForTesting();

			var singleValue = new ClassWithGeneric<int>();
			singleValue.Description = "testtest345";

			var list = new LinkedList<IDescribable>();
			list.AddFirst(singleValue);

			var result = serializer.Serialize(list);

			var deserializedValue = serializer.Deserialize<LinkedList<IDescribable>>(result);

			deserializedValue.First()
			                 .Description.Should()
			                 .Be("testtest345");
		}

		[Fact]
		void GenericDictionaryValues()
		{
			var serializer = new ConfigurationContainer()
			                 .Create()
			                 .ForTesting();

			var singleValue = new ClassWithGeneric<int>();
			singleValue.Description = "testtest345";

			IDictionary<string, IDescribable> list = new Dictionary<string, IDescribable>()
			{
				{"key", singleValue}
			};

			var result = serializer.Serialize(list);

			var deserializedValue = serializer.Deserialize<Dictionary<string, IDescribable>>(result);

			deserializedValue.First()
			                 .Value.Description.Should()
			                 .Be("testtest345");
		}
	}
}