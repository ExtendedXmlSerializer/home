using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue248Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer().EnableAllConstructors()
			                                             .EnableParameterizedContent()
			                                             .UseOptimizedNamespaces()
			                                             .EnableReferences()
			                                             .UseOptimizedNamespaces()
			                                             .Create()
			                                             .ForTesting();

			var instance = new List<ISite<IContext>> {new SpecificClass()};
			serializer.Assert(instance,
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><List xmlns:ns1=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:arguments=""ns1:Issue248Tests-ISite[ns1:Issue248Tests-IContext]"" xmlns=""https://extendedxmlserializer.github.io/system""><Capacity>4</Capacity><ns1:Issue248Tests-SpecificClass><Context><Value>My context</Value></Context></ns1:Issue248Tests-SpecificClass></List>");
			serializer.Cycle(instance)
			          .Should().BeEquivalentTo(instance);
		}

		public interface ISite<out TSiteContext> where TSiteContext : class, IContext
		{
			TSiteContext Context { get; }
		}

		public interface IContext
		{
			string Value { get; }
		}

		public class SpecificClass : SiteBase<Context>
		{
			// ReSharper disable once VirtualMemberCallInConstructor
			public SpecificClass() => Context = new Context();

			public override Context Context { get; set; }
		}

		public abstract class SiteBase<TSiteContext> : ISite<TSiteContext> where TSiteContext : class, IContext
		{
			// ReSharper disable once MemberHidesStaticFromOuterClass
			public abstract TSiteContext Context { get; set; }
		}

		public class Context : IContext
		{
			public string Value { get; set; } = "My context";
		}
	}
}