using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue397Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().UseAutoFormatting()
			                                             .UseOptimizedNamespaces()
			                                             .EnableImplicitTyping(typeof(MainDTO),
			                                                                   typeof(SubDTO), typeof(SubSubDTO))
			                                             .Type<SubSubDTO>()
			                                             .EnableReferences(definition => definition.Id)
			                                             .Create()
			                                             .ForTesting();

			var instance = new MainDTO();
			var cycled   = serializer.Cycle(instance);
			cycled.Sub1.SubSub1.Should().BeSameAs(cycled.Sub2.SubSub1);
			cycled.Sub1.SubSub1.Should().BeSameAs(cycled.Sub3.SubSub1);
		}

		public class MainDTO
		{
			public SubDTO Sub1 { get; set; } = new SubDTO();
			public SubDTO Sub2 { get; set; } = new SubDTO();
			public SubDTO Sub3 { get; set; } = new SubDTO();
		}

		public class SubDTO
		{
			public SubSubDTO SubSub1 { get; set; } = SubSubDTO.NullObject;
		}

		public class SubSubDTO
		{
			public static SubSubDTO NullObject { get; } = new SubSubDTO();

			public string Id { get; set; } = Guid.NewGuid().ToString();
		}
	}
}