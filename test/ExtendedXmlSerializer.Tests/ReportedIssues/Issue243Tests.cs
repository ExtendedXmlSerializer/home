using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using System.Collections.Generic;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue243Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer()
			                 .EnableImplicitTyping(typeof(MediaContainer), typeof(MediaItem))
			                 .Create();

			var subject = new MediaContainer
			{
				Size = 1,
				Items = new List<MediaItem>
				{
					new MediaItem {Id = "1", Name = "Name1"}, new MediaItem {Id = "2", Name = "Name2"}
				}
			};
			// language=XML
			const string content =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
<Issue243Tests-MediaContainer size=""1"">
	<MediaItem>
		<Issue243Tests-MediaItem Name=""Name1"" Id=""1"" />
		<Issue243Tests-MediaItem Name=""Name2"" Id=""2"" />
	</MediaItem>
</Issue243Tests-MediaContainer>";

			var result = serializer.Deserialize<MediaContainer>(content);
			result.ShouldBeEquivalentTo(subject);
			result.Items.Should()
			      .HaveCount(2);
		}

		public class MediaContainer
		{
			[XmlAttribute("size")]
			public int Size { get; set; }

			[XmlElement("MediaItem")]
			public List<MediaItem> Items { get; set; }
		}

		public class MediaItem
		{
			[XmlAttribute]
			public string Name { get; set; }

			[XmlAttribute]
			public string Id { get; set; }
		}
	}
}