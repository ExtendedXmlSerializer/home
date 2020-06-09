using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using JetBrains.Annotations;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue401Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().UseOptimizedNamespaces()
			                                             .EnableImplicitTyping(typeof(MainDTO), typeof(SubSubDTO))
			                                             .Type<SubSubDTO>()
			                                             .EnableReferences()
			                                             .Create()
			                                             .ForTesting();

			var instance = new MainDTO();
			serializer.Assert(instance, $@"<?xml version=""1.0"" encoding=""utf-8""?><Issue401Tests-MainDTO xmlns:exs=""https://extendedxmlserializer.github.io/v2""><Sub1 exs:identity=""1""><Id>{SubSubDTO.NullObject.Id}</Id></Sub1><Sub2 exs:reference=""1"" /><Sub3 exs:reference=""1"" /></Issue401Tests-MainDTO>");
		}

		[Fact]
		public void VerifySealed()
		{
			var serializer = new ConfigurationContainer().UseOptimizedNamespaces()
			                                             .EnableImplicitTyping(typeof(SealedMainDTO), typeof(SealedSubSubDTO))
			                                             .Type<SealedSubSubDTO>()
			                                             .EnableReferences()
			                                             .Create()
			                                             .ForTesting();

			var instance = new SealedMainDTO();
			serializer.Assert(instance, $@"<?xml version=""1.0"" encoding=""utf-8""?><Issue401Tests-SealedMainDTO xmlns:exs=""https://extendedxmlserializer.github.io/v2""><Sub1 exs:identity=""1""><Id>{SealedSubSubDTO.NullObject.Id}</Id></Sub1><Sub2 exs:reference=""1"" /><Sub3 exs:reference=""1"" /></Issue401Tests-SealedMainDTO>");
		}

		class MainDTO
		{
			[UsedImplicitly]
			public SubSubDTO Sub1 { get; set; } = SubSubDTO.NullObject;

			[UsedImplicitly]
			public SubSubDTO Sub2 { get; set; } = SubSubDTO.NullObject;

			[UsedImplicitly]
			public SubSubDTO Sub3 { get; set; } = SubSubDTO.NullObject;
		}

		class SubSubDTO
		{
			public static SubSubDTO NullObject { get; } = new SubSubDTO();

			public string Id { get; set; } = Guid.NewGuid().ToString();
		}

		sealed class SealedMainDTO
		{
			[UsedImplicitly]
			public SealedSubSubDTO Sub1 { get; set; } = SealedSubSubDTO.NullObject;

			[UsedImplicitly]
			public SealedSubSubDTO Sub2 { get; set; } = SealedSubSubDTO.NullObject;

			[UsedImplicitly]
			public SealedSubSubDTO Sub3 { get; set; } = SealedSubSubDTO.NullObject;
		}

		sealed class SealedSubSubDTO
		{
			public static SealedSubSubDTO NullObject { get; } = new SealedSubSubDTO();

			public string Id { get; set; } = Guid.NewGuid().ToString();
		}
	}
}
