using ExtendedXmlSerializer.Configuration;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue141Tests
	{
		[Fact]
		public void ShouldPreserveNullValueIfDefaultIsNotNull()
		{
			var serializer = new ConfigurationContainer().Emit(EmitBehaviors.WhenModified)
			                                             .Create();

			var xml          = serializer.Serialize(new ClassWithDefaultString {Name = null});
			var deserialized = serializer.Deserialize<ClassWithDefaultString>(xml);

			deserialized.Name.Should()
			            .BeNull();
		}

		[Fact]
		public void ShouldPreserveNullStringValueIfDefaultIsNotNull()
		{
			var config = new ConfigurationContainer();
			config.Emit(EmitBehaviors.WhenModified);
			var serializer = config.Create();

			var xml          = serializer.Serialize(new ClassWithDefaultString() {Name = null, SubNode = null});
			var deserialized = serializer.Deserialize<ClassWithDefaultString>(xml);

			deserialized.Name.Should()
			            .BeNull();
		}

		[Fact]
		public void ShouldPreserveNullObjectValueIfDefaultIsNotNull()
		{
			var config = new ConfigurationContainer();
			config.Emit(EmitBehaviors.WhenModified);
			var serializer = config.Create();

			var xml          = serializer.Serialize(new ClassWithDefaultString {Name = null, SubNode = null});
			var deserialized = serializer.Deserialize<ClassWithDefaultString>(xml);

			deserialized.SubNode.Should()
			            .BeNull();
		}

		[Fact]
		public void ShouldPreserveNullObjectValueIfEmitWhenReturnsTrue()
		{
			var serializer = new ConfigurationContainer().Emit(EmitBehaviors.WhenModified)
			                                             .Type<ClassWithDefaultString>()
			                                             .Member(x => x.SubNode)
			                                             .EmitWhen(x => true)
			                                             .Create();

			var xml          = serializer.Serialize(new ClassWithDefaultString() {Name = null, SubNode = null});
			var deserialized = serializer.Deserialize<ClassWithDefaultString>(xml);

			deserialized.SubNode.Should()
			            .BeNull();
		}

		public class ClassWithDefaultString
		{
			public string Name { get; set; } = "Unnamed";

			public string Attribute { get; set; } = "Unset";
			public SubClassWithDefautString SubNode { get; set; } = new SubClassWithDefautString();
		}

		public class SubClassWithDefautString
		{
			public string Name { get; set; } = "UnnamedSubclass";
		}
	}
}