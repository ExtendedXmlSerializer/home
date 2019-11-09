using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue213Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new TestClass();
			new ConfigurationContainer().Emit(EmitBehaviors.Always)
			                            .Create()
			                            .Cycle(instance)
			                            .TestClass2
			                            .Should()
			                            .BeNull();
		}

		public class TestClass
		{
			public TestClass2 TestClass2 { get; [UsedImplicitly] set; }
		}

//     vvvvvvvv
		public abstract class TestClass2
		{
			public string Foo { get; set; }
		}
	}
}