using ExtendedXmlSerializer.Configuration;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue521Tests
	{
		[Fact]
		public void Verify()
		{
			const string document =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
<Issue521Tests-Test>
  <Value1>11</Value1>
  <Item1>
	<Value>1</Value>
  </Item1>
  <Value2>22</Value2>
  
</Issue521Tests-Test>";

			var serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(Test))
														 .WithUnknownContent()
														 .Continue()
														 //.Call(_ => Console.WriteLine("Unknown content: " + _.Name))
														 .Create();
			var instance = serializer.Deserialize<Test>(document);
			instance.Value2.Should().Be(22);
		}

		[Fact]
		public void VerifyComprehensive()
		{
			const string document =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
<Issue521Tests-Test>
   <Value1>11</Value1>
  <Item1>
	<Value>1</Value>
  </Item1>
  <Value2>22</Value2>
  <Items2>
	<Capacity>4</Capacity>
	<int xmlns=""https://extendedxmlserializer.github.io/system"">1</int>
	<int xmlns=""https://extendedxmlserializer.github.io/system"">2</int>
	<int xmlns=""https://extendedxmlserializer.github.io/system"">3</int>
  </Items2>
  <Value3>33</Value3>
  <Items3>
	<Capacity>4</Capacity>
	<Item>
	  <Value>1</Value>
	</Item>
	<Item>
	  <Value>2</Value>
	</Item>
	<Item>
	  <Value>3</Value>
	</Item>
  </Items3>
  <Value4>44</Value4>  
</Issue521Tests-Test>";

			var serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(Test), typeof(Item))
														 .WithUnknownContent()
														 .Continue()
														 //.Call(_ => Console.WriteLine("Unknown content: " + _.Name))
														 .Create();
			var instance = serializer.Deserialize<Test>(document);
			instance.Value2.Should().Be(22);
			instance.Value3.Should().Be(33);
			instance.Value4.Should().Be(44);
		}

		public class Test
		{
			public int Value1 { get; set; }

			//public Item Item1 { get; set; }
			public int Value2 { get; set; }

			//public List<int> Items2 { get; set; }
			public int Value3 { get; set; }

			//public List<Item> Items3 { get; set; }
			public int Value4 { get; set; }
		}

		public class Item
		{
			public int Value { get; set; }
		}
	}
}