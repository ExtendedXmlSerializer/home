using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue190Tests
	{
		[Fact]
		void VerifyThrows()
		{


			var action = new Action(() =>
			                        {
				                        new ConfigurationContainer().Create()
				                                                    .Serialize(new TestClass());
			                        });
			action.ShouldThrow<Exception>();

		}

		[Fact]
		void Verify()
		{
			new ConfigurationContainer().EnableReferences()
			                            .Create()
			                            .Serialize(new TestClass());
		}

		public class TestClass
		{
			public TestClass()
			{
				Id = 1;
				var test = new TestClass2();
				Test1 = test;
				Test2 = test;
			}

			public int Id { get; set; }

			public TestClass2 Test1 { get; set; }

			public TestClass2 Test2 { get; set; }
		}

		public class TestClass2
		{
			public int Id { get; set; }
		}
	}
}