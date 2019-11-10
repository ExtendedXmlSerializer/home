using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue200Tests
	{
		[Fact]
		void Verify()
		{
			var properties    = new MemberSpecification<PropertyInfo>(AllowPrivateSetters.Default);
			var specification = new MetadataSpecification(properties, DefaultMetadataSpecification.Field);
			var extensions =
				new DefaultExtensions(specification, DeclaredMemberNames.Default, DefaultMemberOrder.Default).ToArray();
			var container = new ConfigurationContainer(extensions).Create()
			                                                      .ForTesting();

			var subject = new PrivateSettablePropertySubject("Hello World!");
			container.Cycle(subject)
			         .Should().BeEquivalentTo(subject);
		}

		sealed class PrivateSettablePropertySubject
		{
			[UsedImplicitly]
			public PrivateSettablePropertySubject() : this("Default Message") {}

			public PrivateSettablePropertySubject(string message) => Message = message;

			public string Message { [UsedImplicitly] get; private set; }
		}

		sealed class AllowPrivateSetters : ISpecification<PropertyInfo>
		{
			public static AllowPrivateSetters Default { get; } = new AllowPrivateSetters();

			AllowPrivateSetters() {}

			public bool IsSatisfiedBy(PropertyInfo parameter)
			{
				var getMethod = parameter.GetGetMethod(true);
				var result = parameter.CanRead && !getMethod.IsStatic && getMethod.IsPublic &&
				             parameter.GetIndexParameters()
				                      .Length <= 0;
				return result;
			}
		}
	}
}