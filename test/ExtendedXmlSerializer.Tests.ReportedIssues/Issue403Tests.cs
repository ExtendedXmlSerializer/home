using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue403Tests
	{
		[Fact]
		public void VerifyImplicit()
		{
			var serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(Parent), typeof(Child))
			                                             .UseOptimizedNamespaces()
			                                             .Type<Child>()
			                                             .EnableReferences()
			                                             .Create()
			                                             .ForTesting();

			var child = new Child {Id = Guid.Parse("{64D96027-94CD-4EA4-B102-4EED74BF53B0}"), Message = "Hello World!"};
			var instance = new Parent {First = child, Second = child};

			serializer.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue403Tests-Parent xmlns:exs=""https://extendedxmlserializer.github.io/v2""><First exs:identity=""1""><Id>64d96027-94cd-4ea4-b102-4eed74bf53b0</Id><Message>Hello World!</Message></First><Second exs:reference=""1"" /></Issue403Tests-Parent>");

		}

		[Fact]
		public void VerifyExplicit()
		{
			var serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(Parent), typeof(Child))
			                                             .UseOptimizedNamespaces()
			                                             .Type<Child>()
			                                             .EnableReferences(x => x.Id)
			                                             .Create()
			                                             .ForTesting();

			var child = new Child {Id = Guid.Parse("{64D96027-94CD-4EA4-B102-4EED74BF53B0}"), Message = "Hello World!"};
			var instance = new Parent {First = child, Second = child};

			serializer.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue403Tests-Parent xmlns:exs=""https://extendedxmlserializer.github.io/v2""><First Id=""64d96027-94cd-4ea4-b102-4eed74bf53b0""><Message>Hello World!</Message></First><Second exs:entity=""64d96027-94cd-4ea4-b102-4eed74bf53b0"" /></Issue403Tests-Parent>");

		}

		public class Parent
		{
			public Child First { get; set; }
			public Child Second { get; set; }
		}

		public class Child
		{
			public Guid Id { get; set; }
			public string Message { get; set; }
		}
	}
}