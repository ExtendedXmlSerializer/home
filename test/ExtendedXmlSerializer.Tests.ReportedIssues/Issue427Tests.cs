using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Linq;
using Xunit;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue427Tests
	{
		[Fact]
		public void Verify()
		{
			var instance = new Subject("Hello World", 123);
			var serializer = new ConfigurationContainer().EnableParameterizedContent().Create().ForTesting();

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);

			var element = new Base[] {instance};
			serializer.Cycle(element).Should().BeEquivalentTo(element.Cast<Subject>());

		}

		[Fact]
		public void VerifyBasic()
		{
			var instance   = new BasicSubject{Name = "Hello World", Number = 123};
			var serializer = new ConfigurationContainer().Create().ForTesting();

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);

			var element = new Basic[] {instance};
			serializer.Cycle(element).Should().BeEquivalentTo(element.Cast<Basic>());

		}

		[Fact]
		public void VerifyReported()
		{
			var config = new NewFieldInfo(FieldType.Int)
			{
				Name       = "abc",
				TargetName = "edf"
			};

			var serializer = new ConfigurationContainer().EnableParameterizedContentWithPropertyAssignments()
			                                             .Create()
			                                             .ForTesting();

			var instance = new NewTypeInfo[] {config};
			serializer.Cycle(instance).Should().BeEquivalentTo(instance.Cast<NewFieldInfo>());
		}

		class BasicSubject : Basic
		{
			public string Name { get; set; }
		}

		class Basic
		{
			public int Number { get; set; }
		}

		class Subject : Base
		{
			public Subject(string name, int number) : base(number) => Name = name;

			public string Name { get; }
		}

		class Base
		{
			public Base(int number) => Number = number;

			public int Number { get; }
		}

		public enum FieldType
		{
			Int,
			Bool
		}

		public class NewFieldInfo : NewTypeInfo
		{
			public NewFieldInfo(FieldType type) : base(type) {}

			public string Description { get; set; } = null;

			public bool IsRequired { get; set; }

			public string TargetName { get; set; }
		}

		public class NewTypeInfo
		{
			public NewTypeInfo(FieldType type)
			{
				Type = type;
			}

			public FieldType Type { get; }

			public string Name { get; set; }
		}
	}
}