using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue144Tests
	{
		[Fact]
		public void Verify()
		{
			var sut = new ConfigurationContainer().EnableParameterizedContent()
			                                      .Create()
			                                      .ForTesting();

			var subject = new Subject("Testing", new Amounts{ {"First", 1234 } });

			sut.Cycle(subject).Should().BeEquivalentTo(subject);
		}

		sealed class Subject
		{
			public Subject(string name, Amounts amounts, double factor = .95)
			{
				Name = name;
				Amounts = amounts;
				Factor = factor;
			}

			public string Name { [UsedImplicitly] get; }


			public Amounts Amounts { [UsedImplicitly] get; }
			public double Factor { [UsedImplicitly] get; }
		}

		sealed class Amounts : Dictionary<string, decimal>
		{
			public Amounts(double padding = .95) => Padding = padding;

			public double Padding { [UsedImplicitly] get; }
		}
	}
}