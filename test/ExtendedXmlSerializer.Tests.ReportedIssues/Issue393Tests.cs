using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue393Tests
	{
		[Fact]
		void Verify()
		{
			const string content = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<pin>
  <id>1309801580</id>
  <code>GBCT</code>
  <expires-at>2020-05-10T18:52:14Z</expires-at>
  <user-id>231221</user-id>
  <client-identifier>8c89f40c-e1f3-45e1-95bc-a45a047ce77f</client-identifier>
  <trusted>false</trusted>
  <auth-token>z6ZM5jeSP19WroEQc2Xy</auth-token>
  <auth_token>z6ZM5jeSP19WroEQc2Xy</auth_token>
</pin>";
			var support = new ConfigurationContainer().EnableImplicitTyping(typeof(Pin))
			                                          .WithUnknownContent()
			                                          .Continue()
			                                          .Create()
			                                          .ForTesting();
			var stored = support.Deserialize<Pin>(content);

			var expectation = new Pin
			{
				ID = 1309801580, Code = "GBCT",
				ExpiresAtUTC = DateTimeOffset.Parse("2020-05-10T18:52:14Z").UtcDateTime,
				AuthToken = "z6ZM5jeSP19WroEQc2Xy", ClientIdentifier = new Guid("8c89f40c-e1f3-45e1-95bc-a45a047ce77f")
			};
			stored.Should().BeEquivalentTo(expectation);
		}

		[XmlRoot("pin")]
		public sealed class Pin
		{
			[XmlElement("id")]
			public int ID { get; set; }

			[XmlElement("code")]
			public string Code { get; set; }

			[XmlElement("expires-at")]
			public DateTime ExpiresAtUTC { get; set; }

			[XmlElement("auth-token")]
			public string AuthToken { get; set; }

			[XmlElement("client-identifier")]
			public Guid ClientIdentifier { get; set; }
		}
	}
}