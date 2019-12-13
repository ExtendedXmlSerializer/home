using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue346Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new MyClass
				{SomeAttributeProperty = "Hello World!", SomeOtherProperty = new DateTime(2019, 12, 13)};

			new ConfigurationContainer().ForTesting()
			                            .Cycle(instance)
			                            .Should()
			                            .BeEquivalentTo(instance);
		}

		[Fact]
		void VerifyInstance()
		{
			var instance = new MyClass
			{
				SomeAttributeProperty = "Hello World!",
				SomeOtherProperty     = new MyClass {SomeAttributeProperty = "Hello World Again!"}
			};

			new ConfigurationContainer().ForTesting()
			                            .Cycle(instance)
			                            .Should()
			                            .BeEquivalentTo(instance);
		}

		public class MyClass
		{
			public string SomeAttributeProperty { get; set; }

			public object SomeOtherProperty { get; set; }
		}
	}
}