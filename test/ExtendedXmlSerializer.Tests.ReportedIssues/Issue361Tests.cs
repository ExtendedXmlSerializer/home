using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue361Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer().EnableParameterizedContentWithPropertyAssignments()
			                                             .Type<AdornedImage>()
			                                             .CustomSerializer(new AdornedImageSerializer())
			                                             .Create()
			                                             .ForTesting();

			serializer.Assert(new AdornedImage(),
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue361Tests-AdornedImage xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues"" />");
		}

		class AdornedImageSerializer : IExtendedXmlCustomSerializer<AdornedImage>
		{
			public AdornedImage Deserialize(XElement xElement)
			{
				return new AdornedImage();
			}

			public void Serializer(XmlWriter xmlWriter, AdornedImage obj) {}
		}

		class AdornedImage
		{
			public vector Vector { get; set; }

			public AdornedImage()
			{
				var data = new length(23);
				Vector = new vector(data, data);
			}
		}

		class length
		{
			public length(int value)
			{
				Value = value;
			}

			public int Value { get; }
		}

		private struct vector
		{
			public vector(length l1, length l2)
			{
				L1 = l1;
				L2 = l2;
			}

			public length L1 { get; }
			public length L2 { get; }
		}
	}
}