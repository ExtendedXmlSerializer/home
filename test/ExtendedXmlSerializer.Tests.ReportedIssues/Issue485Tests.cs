﻿using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
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

		[Fact]
		public void VerifySortedSet()
		{
			var instance = new[] { 1, 2, 3, 4 }.ToImmutableSortedSet();

			var serializer = new ConfigurationContainer().UseAutoFormatting()
			                                             .UseOptimizedNamespaces()
			                                             .EnableParameterizedContentWithPropertyAssignments()
			                                             .Create()
			                                             .ForTesting();
			serializer.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><ImmutableSortedSet xmlns:sys=""https://extendedxmlserializer.github.io/system"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:arguments=""sys:int"" xmlns=""clr-namespace:System.Collections.Immutable;assembly=System.Collections.Immutable""><sys:int>1</sys:int><sys:int>2</sys:int><sys:int>3</sys:int><sys:int>4</sys:int></ImmutableSortedSet>");
			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		[Fact]
		public void VerifyDictionary()
		{
			var instance = new Dictionary<string, string>{["Hello"] = "World"}.ToImmutableDictionary();

			var serializer = new ConfigurationContainer().UseAutoFormatting()
			                                             .UseOptimizedNamespaces()
			                                             .EnableParameterizedContentWithPropertyAssignments()
			                                             .Create()
			                                             .ForTesting();

			serializer.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><ImmutableDictionary xmlns:sys=""https://extendedxmlserializer.github.io/system"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:arguments=""sys:string,sys:string"" xmlns=""clr-namespace:System.Collections.Immutable;assembly=System.Collections.Immutable""><sys:Item Key=""Hello"" Value=""World"" /></ImmutableDictionary>");

			var dictionary = serializer.Cycle(instance);
			dictionary.Should().BeEquivalentTo(instance);

			dictionary["Hello"].Should().Be(instance["Hello"]);
		}
	}
}