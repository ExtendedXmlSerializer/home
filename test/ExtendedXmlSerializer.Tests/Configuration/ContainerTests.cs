using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace ExtendedXmlSerializer.Tests.Configuration
{
	public sealed class ContainerTests
	{
		[Fact]
		public void Verify()
		{
			new ConfigurationContainer().ToSupport().Cycle(6776);
		}

		[Fact]
		public void VerifyTypes()
		{
			var container = new ConfigurationContainer();
			container.Types()
			         .Should()
			         .BeEmpty();

			container.Type<Subject>()
			         .Should()
			         .BeSameAs(container.Type<Subject>());

			container.Types()
			         .Should()
			         .ContainSingle();
		}

		[Fact]
		public void VerifyMembers()
		{
			var container = new ConfigurationContainer();
			var type = container.Type<Subject>();

			type.Members()
			    .Should()
			    .BeEmpty();

			var member = type.Member(x => x.Message);
			member.Should()
			      .BeSameAs(type.Member(x => x.Message));

			type.Members()
			    .Should()
			    .ContainSingle();
		}

		[Fact]
		public void VerifyClass()
		{
			new ConfigurationContainer().ToSupport()
			                            .Cycle(new Subject {Message = "Hello World!"});
		}

		[Fact]
		public void VerifyArray()
		{
			new ConfigurationContainer().ToSupport()
			                            .Cycle(new[]{1, 2, 3});
		}

		[Fact]
		public void VerifyCollection()
		{
			new ConfigurationContainer().ToSupport()
			                            .Cycle(new List<string>{ "Hello", "World"});
		}

		[Fact]
		public void VerifyDictionary()
		{
			new ConfigurationContainer().ToSupport()
			                            .Cycle(new Dictionary<string, int>{ {"Hello", 1}, {"World!", 2}});
		}


		[Fact]
		public void VerifyNullable()
		{
			var support = new ConfigurationContainer().ToSupport();
			support.Cycle(new int?(6776));
			support.Cycle((int?)null);
		}



		sealed class Subject
		{
			public string Message { get; set; }
		}
	}
}
