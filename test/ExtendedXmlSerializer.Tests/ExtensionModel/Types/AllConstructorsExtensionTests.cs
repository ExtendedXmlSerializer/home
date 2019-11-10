using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

// ReSharper disable All

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Types
{
	public class AllConstructorsExtensionTests
	{
		[Fact]
		public void PrivateConstructor()
		{
			var container = new ConfigurationContainer().EnableAllConstructors()
			                                            .Create();
			var support  = new SerializationSupport(container);
			var instance = Subject.Create("Hello World from Private Constructor (hopefully)!");
			support.Cycle(instance)
			       .Should().BeEquivalentTo(instance);
		}

		sealed class Subject
		{
			public static Subject Create(string message) => new Subject {PropertyName = message};

			Subject() {}

			public string PropertyName { get; [UsedImplicitly] set; }
		}
	}
}