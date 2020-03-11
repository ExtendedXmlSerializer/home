using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue374Tests
	{
		[Fact]
		void Verify()
		{
			var properties    = new MemberSpecification<PropertyInfo>(AllowPrivateSetters.Default);
			var specification = new MetadataSpecification(properties, DefaultMetadataSpecification.Field);
			var extensions =
				new DefaultExtensions(specification, DeclaredMemberNames.Default, DefaultMemberOrder.Default).ToArray();
			var support = new ConfigurationContainer(extensions).ForTesting();
			var instance = new Dto();
			instance.Names.Add("Hello");
			instance.Names.Add("World");
			support.Cycle(instance).Should().BeEquivalentTo(instance);
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

		sealed class Dto
		{
			Collection<string> _names;

			public Dto()
			{
				_names = new Collection<string>();
			}

			public Collection<string> Names
			{
				get => _names;
				private set => _names = value;
			}
		}
	}
}