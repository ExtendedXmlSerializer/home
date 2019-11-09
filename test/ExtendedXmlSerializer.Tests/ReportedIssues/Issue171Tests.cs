using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue171Tests
	{
		[Fact]
		public void Verify()
		{
			new ConfigurationContainer()
				.Ignore(typeof(List<>).GetProperty(nameof(List<object>.Capacity)))
				.ForTesting()
				.Assert(new List<int>(),
				        @"<?xml version=""1.0"" encoding=""utf-8""?><List xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:arguments=""int"" xmlns=""https://extendedxmlserializer.github.io/system"" />");
		}

		[Fact]
		public void VerifySpecific()
		{
			new ConfigurationContainer().Type<List<int>>()
			                            .Member(x => x.Capacity)
			                            .Ignore()
			                            .ForTesting()
			                            .Assert(new List<int>(),
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><List xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:arguments=""int"" xmlns=""https://extendedxmlserializer.github.io/system"" />");
		}
	}
}