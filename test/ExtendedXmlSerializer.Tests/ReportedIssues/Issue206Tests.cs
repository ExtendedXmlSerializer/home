using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue206Tests
	{
		[Fact]
		void Verify()
		{
			var support = new ConfigurationContainer().EnableImplicitTypingFromPublicNested<Issue206Tests>()
			                                          .EnableClassicListNaming()
			                                          .Create()
			                                          .ForTesting();

			var subject = new[]
			{
				new TravelFile
				{
					Name = "Hello World!", Participants = new[]
					{
						new Participant {ParticipantId = 679556, Name = "xxxx"},
						new Participant {ParticipantId = 679557, Name = "xxx"}
					}.ToList()
				}
			};
			support
				.Deserialize<TravelFile[]
				>(@"<?xml version=""1.0"" encoding=""utf-8""?><ArrayOfIssue206Tests-TravelFile xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" ><Issue206Tests-TravelFile Name=""Hello World!""><Participants>
		 <Issue206Tests-Participant>
			<ParticipantId>679556</ParticipantId>
			<Name>xxxx</Name>
		 </Issue206Tests-Participant>
		 <Issue206Tests-Participant>
			<ParticipantId>679557</ParticipantId>
			<Name>xxx</Name>
</Issue206Tests-Participant>
	  </Participants></Issue206Tests-TravelFile></ArrayOfIssue206Tests-TravelFile>")
				// ReSharper disable once CoVariantArrayConversion
				.Should().BeEquivalentTo(subject);
		}

		[Fact]
		void VerifyExtension()
		{
			var support = new ConfigurationContainer().EnableClassicListNaming()
			                                          .Create()
			                                          .ForTesting();
			var subject = new[]
			{
				new TravelFile
				{
					Name = "Hello World!", Participants = new[]
					{
						new Participant {ParticipantId = 679556, Name = "xxxx"},
						new Participant {ParticipantId = 679557, Name = "xxx"}
					}.ToList()
				}
			};

			support.Cycle(subject)
			       // ReSharper disable once CoVariantArrayConversion
			       .Should().BeEquivalentTo(subject);
		}

		public sealed class TravelFile
		{
			public List<Participant> Participants { [UsedImplicitly] get; set; }

			public string Name { [UsedImplicitly] get; set; }
		}

		public class Participant
		{
			public int ParticipantId { [UsedImplicitly] get; set; }

			public string Name { [UsedImplicitly] get; set; }
		}
	}
}