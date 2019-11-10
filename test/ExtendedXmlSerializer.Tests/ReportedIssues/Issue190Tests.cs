using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using JetBrains.Annotations;
using System;
using Xunit;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

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
			action.Should().Throw<Exception>();
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

			public int Id { [UsedImplicitly] get; set; }

			public TestClass2 Test1 { [UsedImplicitly] get; set; }

			public TestClass2 Test2 { [UsedImplicitly] get; set; }
		}

		public class TestClass2
		{
			public int Id { get; set; }
		}
	}
}