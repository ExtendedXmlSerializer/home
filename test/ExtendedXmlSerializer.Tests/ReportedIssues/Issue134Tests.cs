using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue134Tests
	{
		public struct SomeKey
		{
			public string Message { get; }

			public SomeKey(string message) => Message = message;
		}

		[Fact]
		public void Fix()
		{
			var dictionary = new Dictionary<SomeKey, string> {{new SomeKey("Hello"), "World!"}};
			var serializer = new ConfigurationContainer().UseOptimizedNamespaces()
			                                             .UseAutoFormatting()
			                                             .EnableParameterizedContent()
			                                             .Create();
			var contents = new SerializationSupport(serializer).Cycle(dictionary);

			contents.Keys.Single()
			        .Message.Should()
			        .Be("Hello");
			contents.Values.Single()
			        .Should()
			        .Be("World!");
		}
	}
}