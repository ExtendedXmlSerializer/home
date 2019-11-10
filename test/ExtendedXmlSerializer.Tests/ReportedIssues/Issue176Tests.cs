using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue176Tests
	{
		[Fact]
		public void Verify()
		{
			var test = new SerializableProject
			{
				Cases = new List<Case>
				{
					new SimpleCase
					{
						Name       = "Simple",
						Number     = 1,
						ModesCount = 2,
						Records = new List<LoadRecord>
						{
							new LoadRecord
							{
								Description = "DO NOT WORK :("
							}
						}
					},
					new CaseCombination()
					{
						Number = 2,
						Name   = "A",
						label  = "C"
					}
				}
			};
			var support = new ConfigurationContainer().ForTesting();

			support.Cycle(test)
			       .Should().BeEquivalentTo(test, options => options.RespectingRuntimeTypes());
		}

		[Fact]
		public void VerifyOptimized()
		{
			var test = new SerializableProject
			{
				Cases = new List<Case>
				{
					new SimpleCase
					{
						Name       = "Simple",
						Number     = 1,
						ModesCount = 2,
						Records = new List<LoadRecord>
						{
							new LoadRecord
							{
								Description = "DO NOT WORK :("
							}
						}
					},
					new CaseCombination()
					{
						Number = 2,
						Name   = "A",
						label  = "C"
					}
				}
			};
			var support = new ConfigurationContainer()
			              .UseOptimizedNamespaces() //WITHOUT IT WORKS
			              .ForTesting();

			// ShouldBeEquivalentTo doesn't work
			support.Cycle(test)
			       .Should().BeEquivalentTo(test, options => options.RespectingRuntimeTypes());
		}

		public class Case
		{
			public int Number { [UsedImplicitly] get; set; }
			public string Name { [UsedImplicitly] get; set; }
		}

		public class SimpleCase : Case
		{
			public List<LoadRecord> Records { [UsedImplicitly] get; set; }

			public int ModesCount { [UsedImplicitly] get; set; }
		}

		public class CaseCombination : Case
		{
			public string label { [UsedImplicitly] get; set; }
		}

		public class LoadRecord
		{
			public string Description { [UsedImplicitly] get; set; }
		}

		public class SerializableProject
		{
			public List<Case> Cases { [UsedImplicitly] get; set; }
		}
	}
}