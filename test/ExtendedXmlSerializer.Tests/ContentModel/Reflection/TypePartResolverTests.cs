using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using FluentAssertions;
using System.Reflection;
using Xunit;
using Identity = ExtendedXmlSerializer.ContentModel.Identification.Identity;

// ReSharper disable All

namespace ExtendedXmlSerializer.Tests.ContentModel.Reflection
{
	public class TypePartResolverTests
	{
		[Fact]
		public void Verify()
		{
			var sut = new TypePartResolver(Identities.Default);

			var @class = sut.Get(typeof(Class));
			@class.Dimensions.Should()
			      .BeNull();
			@class.GetArguments()
			      .Should()
			      .BeNull();

			var generic = sut.Get(typeof(Generic<int>));
			generic.Dimensions.Should()
			       .BeNull();
			generic.GetArguments()
			       .Should()
			       .HaveCount(1);

			var generic2 = sut.Get(typeof(Generic<int, bool>));
			generic2.Dimensions.Should()
			        .BeNull();
			generic2.GetArguments()
			        .Should()
			        .HaveCount(2);
		}

		[Fact]
		public void VerifyDimensions()
		{
			var sut = new TypePartResolver(Identities.Default);
			var one = sut.Get(typeof(Class[]));
			one.Dimensions.Should()
			   .BeEquivalentTo(1);
			one.GetArguments()
			   .Should()
			   .BeNull();

			sut.Get(typeof(Class[,]))
			   .Dimensions.Should()
			   .BeEquivalentTo(2);
			sut.Get(typeof(Class[,][][]))
			   .Dimensions.Should()
			   .BeEquivalentTo(2, 1, 1);
			sut.Get(typeof(Class[,][][,,,]))
			   .Dimensions.Should()
			   .BeEquivalentTo(2, 1, 4);
		}

		sealed class Class {}

		sealed class Generic<T> {}

		sealed class Generic<T1, T2> {}

		sealed class Identities : IIdentities
		{
			public static Identities Default { get; } = new Identities();

			Identities() {}

			public IIdentity Get(TypeInfo parameter) => new Identity(parameter.Name, "testing");
		}
	}
}