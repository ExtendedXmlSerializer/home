using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue173Tests
	{
		[Fact]
		public void Verify()
		{
			//int adultC = 1;
			var childC = "COMPLEXCHILDRENDATA";

			var orgC = new Child
			{
				Name    = "Tom",
				Complex = childC,
			};

			var orgA = new Adult
			{
				Name    = "Andy",
				Complex = new[] {1.0, 2.0}
			};

			var container = new ConfigurationContainer().ForTesting();
			container.Cycle(orgC)
			         .Should().BeEquivalentTo(orgC);
			container.Cycle(orgA)
			         .Should().BeEquivalentTo(orgA);
		}

		public abstract class Person
		{
			public string Name { [UsedImplicitly] get; set; }
			public abstract object Complex { get; set; }
		}

		public class Child : Person
		{
			string _complex;

			public override object Complex
			{
				get => _complex;
				set => _complex = (string)value;
			}
		}

		public class Adult : Person
		{
			//int IDs;
			double[] IDs;

			public override object Complex
			{
				get => IDs;
				//set { IDs = (int)value; }
				set => IDs = (double[])value;
			}
		}
	}
}