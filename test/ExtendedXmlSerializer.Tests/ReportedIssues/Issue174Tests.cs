using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue174Tests
	{
		[Fact]
		public void Verify()
		{
			var support = new ConfigurationContainer().ForTesting();
			support.Assert(SessionTypeEnum.MyTrainingList, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue174Tests-SessionTypeEnum xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"">My Training List</Issue174Tests-SessionTypeEnum>");

			support.Assert(new Subject{Enum = SessionTypeEnum.ServiceList},
			               @"<?xml version=""1.0"" encoding=""utf-8""?><Issue174Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Enum>Service List</Enum></Issue174Tests-Subject>");

			support.Assert(SessionTypeEnum.NotDecorated, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue174Tests-SessionTypeEnum xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"">NotDecorated</Issue174Tests-SessionTypeEnum>");
		}

		sealed class Subject
		{
			public SessionTypeEnum Enum { get; set; }

		}

		public enum SessionTypeEnum
		{
			/// <remarks />
			[XmlEnum("My Training List")] MyTrainingList,

			/// <remarks />
			[XmlEnum("Training List")] TrainingList,

			/// <remarks />
			[XmlEnum("Service List")]
			ServiceList,

			NotDecorated
		}
	}
}
