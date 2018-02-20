using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using Xunit;
using ExtensionCollection = ExtendedXmlSerializer.Configuration.ExtensionCollection;

namespace ExtendedXmlSerializer.Tests.ExtensionModel
{
	public sealed class DefaultExtensionsTests
	{
		[Fact]
		public void VerifyConvertible()
		{
			new ConfigurationRoot().ToSupport().Cycle(6776);
		}

		[Fact]
		public void VerifyOrphanedPropertyThrows()
		{
			Action action = () => new ConfigurationRoot().Type<int>().Get(OrphanedProperty.Default);
			action.ShouldThrow<InvalidOperationException>();
		}


		[Fact]
		public void VerifyExtensionAndDependency()
		{
			var collection = new ExtensionCollection(DefaultExtensions.Default);
			var root = new ConfigurationRoot(collection);
			root.Extensions.FirstOf<Extension>().Should().BeNull();
			root.Extensions.FirstOf<Dependency>().Should().BeNull();
			root.Type<int>().Get(DeclaredProperty.Default);
			var first = root.Extensions.FirstOf<Extension>();
			first.Should().NotBeNull();
			var second = root.Extensions.FirstOf<Dependency>();
			second.Should().NotBeNull();

			var content = collection.Get(Categories.Content);
			content.Should().Contain(first).And.Contain(second);
		}


		sealed class OrphanedProperty : Property<string>
		{
			public static OrphanedProperty Default { get; } = new OrphanedProperty();
			OrphanedProperty() {}
		}

		[Extension(typeof(Extension))]
		sealed class DeclaredProperty : Property<string>
		{
			public static DeclaredProperty Default { get; } = new DeclaredProperty();
			DeclaredProperty() {}
		}

		[Dependency(typeof(Dependency))]
		sealed class Extension : ISerializerExtension
		{
			public IServiceRepository Get(IServiceRepository parameter)
			{
				return null;
			}

			public void Execute(IServices parameter) {}
		}

		sealed class Dependency : ISerializerExtension
		{
			public IServiceRepository Get(IServiceRepository parameter)
			{
				return null;
			}

			public void Execute(IServices parameter) {}
		}
	}
}