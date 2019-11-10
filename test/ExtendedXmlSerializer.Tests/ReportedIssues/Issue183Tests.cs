using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue183Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer().EnableImplicitTypingFromNested<Issue183Tests>()
			                                             .UseOptimizedNamespaces()
			                                             .Create()
			                                             .ForTesting();
			var instance = new Model {PropertyWithInterface = new Implementation()};

			serializer.Assert(instance,
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue183Tests-Model xmlns:exs=""https://extendedxmlserializer.github.io/v2""><PropertyWithInterface exs:type=""Issue183Tests-Implementation"" /></Issue183Tests-Model>");
		}

		sealed class Model
		{
			public IInterface PropertyWithInterface { [UsedImplicitly] get; set; }
		}

		interface IInterface {}

		sealed class Implementation : IInterface {}
	}
}