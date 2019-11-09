using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using System.Collections.Generic;
using System.Xml.Serialization;
using Xunit;

// ReSharper disable All

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue239Tests
	{
		[Fact]
		void Verify()
		{
			const string content = @"<Issue239Tests-Root Id=""ID1234567891234567891234567891234567"">
  <FirstClass>
	<Value1>1</Value1>
	<Value2>2</Value2>
	<Value3>v02_05_00</Value3>
	<SecondClass>
		<SecondClass>
			<Value4>4</Value4>
			<Value5>19468557</Value5>
		</SecondClass>
	</SecondClass>
  </FirstClass>
</Issue239Tests-Root>";
			var subject = new ConfigurationContainer().EnableImplicitTypingFromPublicNested<Issue239Tests>()
			                                          .Create()
			                                          .Deserialize<Root>(content);
			subject.Id.Should()
			       .Be("ID1234567891234567891234567891234567");
			subject.FirstClass.SecondClass.Only()
			       .Value4.Should()
			       .Be(4);
		}

		public class Root
		{
			public string Id { get; set; }

			public FirstClass FirstClass { get; set; }
		}

		public class FirstClass
		{
			public byte Value1 { get; set; }
			public byte Value2 { get; set; }
			public string Value3 { get; set; }

			[XmlElement("SecondClass")]
			public List<SecondClass> SecondClass { get; set; }
		}

		public class SecondClass
		{
			public byte Value4 { get; set; }
			public string Value5 { get; set; }
		}
	}
}