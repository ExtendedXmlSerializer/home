using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue583Tests
	{
		[Fact]
		public void Verify()
		{
			var inner = new InnerObject { Id = 100 };
			var subject = new RootObject
			{
				Item1 = inner,
				Item2 = inner,
				List  = new List<InnerObject> { inner }
			};

			Action action = () => new ConfigurationContainer().Create().Cycle(subject).Should().BeEquivalentTo(subject);
			action.Should()
			      .Throw<Exception>()
			      .WithMessage("The provided instance of type 'ExtendedXmlSerializer.Tests.ReportedIssues.Issue583Tests+RootObject' contains the same reference multiple times in its graph. While this is technically allowed, it is recommended to instead enable referential support by calling EnableReferences on the ConfigurationContainer. Doing so will ensure that multiple references found in the graph are emitted only once in the serialized document.\r\n\r\nHere is a list of found references:\r\n- ExtendedXmlSerializer.Tests.ReportedIssues.Issue583Tests+InnerObject");
		}

		[Fact]
		public void VerifyOnePropertyAndCollectionThrows()
		{
			var inner = new InnerObject { Id = 100 };
			var subject = new RootObjectOneProperty()
			{
				Item1 = inner,
				List  = new List<InnerObject> { inner }
			};

			Action action = () => new ConfigurationContainer().Create().Cycle(subject).Should().BeEquivalentTo(subject);
			action.Should()
			      .Throw<Exception>()
			      .WithMessage("The provided instance of type 'ExtendedXmlSerializer.Tests.ReportedIssues.Issue583Tests+RootObjectOneProperty' contains the same reference multiple times in its graph. While this is technically allowed, it is recommended to instead enable referential support by calling EnableReferences on the ConfigurationContainer. Doing so will ensure that multiple references found in the graph are emitted only once in the serialized document.\r\n\r\nHere is a list of found references:\r\n- ExtendedXmlSerializer.Tests.ReportedIssues.Issue583Tests+InnerObject");
		}

		[Fact]
		public void VerifyAllowed()
		{
			var inner = new InnerObject { Id = 100 };
			var subject = new RootObject
			{
				Item1 = inner,
				Item2 = inner,
				List  = new List<InnerObject> { inner }
			};

			new ConfigurationContainer().AllowMultipleReferences()
			                            .Create()
			                            .Cycle(subject)
			                            .Should()
			                            .BeEquivalentTo(subject);
		}

		public class RootObject
		{
			public InnerObject Item1 { get; set; }

			public InnerObject Item2 { get; set; }

			public List<InnerObject> List { get; set; } = new();
		}

		public class InnerObject
		{
			public int Id { get; set; }
		}

		public class RootObjectOneProperty
		{
			public InnerObject Item1 { get; set; }

			public List<InnerObject> List { get; set; } = new();
		}
	}
}