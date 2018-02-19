using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Services;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel
{
	public sealed class DeclaredDependenciesTests
	{
		[Fact]
		public void Verify()
		{
			var types = DeclaredDependencies<Extension>.Default.Get();

			types.Should()
			     .Contain(new[] {typeof(Dependency1), typeof(Dependency2), typeof(AnotherDependency)});

		}

		[Fact]
		public void VerifyLoadedExtensions()
		{
			var container = new ConfigurationContainer(new ISerializerExtension[]{});
			container.Extensions.Should().ContainSingle();

			container.Extend<Extension>();
			container.Extensions.ElementAt(1).Should().BeOfType<AnotherDependency>();
			container.Extensions.ElementAt(2).Should().BeOfType<Dependency2>();
			container.Extensions.ElementAt(3).Should().BeOfType<Dependency1>();
			container.Extensions.Last().Should().BeOfType<Extension>();
		}


		[Dependency(typeof(Dependency1))]
		sealed class Extension : ISerializerExtension
		{
			public IServiceRepository Get(IServiceRepository parameter) => throw new NotImplementedException();

			public void Execute(IServices parameter) => throw new NotImplementedException();
		}

		[Dependency(typeof(Dependency2))]
		sealed class Dependency1 : ISerializerExtension
		{
			public IServiceRepository Get(IServiceRepository parameter) => throw new NotImplementedException();

			public void Execute(IServices parameter) => throw new NotImplementedException();
		}

		[Dependency(typeof(Dependency1))]
		[Dependency(typeof(AnotherDependency))]
		sealed class Dependency2 : ISerializerExtension
		{
			public IServiceRepository Get(IServiceRepository parameter) => throw new NotImplementedException();

			public void Execute(IServices parameter) => throw new NotImplementedException();
		}

		[Dependency(typeof(Dependency1))]
		sealed class AnotherDependency : ISerializerExtension
		{
			public IServiceRepository Get(IServiceRepository parameter) => throw new NotImplementedException();

			public void Execute(IServices parameter) => throw new NotImplementedException();
		}
	}
}
