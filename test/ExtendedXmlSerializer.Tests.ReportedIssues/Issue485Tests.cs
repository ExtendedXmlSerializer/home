using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Immutable;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue485Tests
	{
		[Fact]
		public void Verify()
		{
			var instance = new[] { 1, 2, 3, 4 }.ToImmutableList();

			var serializer = new ConfigurationContainer().UseAutoFormatting()
			                                             .UseOptimizedNamespaces()
			                                             .EnableParameterizedContentWithPropertyAssignments()
			                                             .Create()
			                                             .ForTesting();
			serializer.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><ImmutableList xmlns:sys=""https://extendedxmlserializer.github.io/system"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:arguments=""sys:int"" xmlns=""clr-namespace:System.Collections.Immutable;assembly=System.Collections.Immutable""><sys:int>1</sys:int><sys:int>2</sys:int><sys:int>3</sys:int><sys:int>4</sys:int></ImmutableList>");
			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		[Fact]
		public void VerifyHashSet()
		{
			var instance = new[] { 1, 2, 3, 4 }.ToImmutableHashSet();

			var serializer = new ConfigurationContainer().UseAutoFormatting()
			                                             .UseOptimizedNamespaces()
			                                             .EnableParameterizedContentWithPropertyAssignments()
			                                             .Create()
			                                             .ForTesting();
			serializer.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><ImmutableHashSet xmlns:sys=""https://extendedxmlserializer.github.io/system"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:arguments=""sys:int"" xmlns=""clr-namespace:System.Collections.Immutable;assembly=System.Collections.Immutable""><sys:int>1</sys:int><sys:int>2</sys:int><sys:int>3</sys:int><sys:int>4</sys:int></ImmutableHashSet>");
			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}
	}
}