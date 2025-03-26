using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues;

public sealed class Issue644Tests
{
	[Fact]
	public void VerifySingle()
	{
		var sut = new ConfigurationContainer().EnableAllConstructors()
		                                      .UseAutoFormatting()
		                                      .EnableClassicListNaming()
		                                      .UseOptimizedNamespaces()
		                                      .AllowMultipleReferences()
		                                      .EnableXmlText()
		                                      .Create()
		                                      .ForTesting();
		var instance = new ObjectContainer
		{
			Object = new IssueClass<int> { value = 2 }
		};
		sut.Cycle(instance).Should().BeEquivalentTo(instance);
	}

	[Fact]
	public void VerifyDouble()
	{
		var sut = new ConfigurationContainer().EnableAllConstructors()
		                                      .UseAutoFormatting()
		                                      .EnableClassicListNaming()
		                                      .UseOptimizedNamespaces()
		                                      .AllowMultipleReferences()
		                                      .EnableXmlText()
		                                      .Create()
		                                      .ForTesting();
		var instance = new ObjectContainer
		{
			Object = new IssueClass<int, string> { value = 2, other = "Hello world!" }
		};
		sut.Cycle(instance).Should().BeEquivalentTo(instance);
	}

	public class ObjectContainer
	{
		public object Object { get; set; }
	}

	public class IssueClass<T>
	{
		public T value;
	}

	public class IssueClass<T, U> : IssueClass<T>
	{
		public U other;
	}
}