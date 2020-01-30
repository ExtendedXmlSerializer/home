using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.Tests.Support;
using JetBrains.Annotations;
using Xunit;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue361Tests
	{
		[Fact]
		void VerifyCustomSerialization_CircularReferenceInsideObjectGraph_DoesNotThrow()
		{
			var serializer = new ConfigurationContainer().EnableParameterizedContentWithPropertyAssignments()
														 .Type<AdornedImage>()
														 .Register().Serializer().Using(new AdornedImageSerializer())
			                                             .Create()
			                                             .ForTesting();

			var image = new AdornedImage();

			serializer.Assert(new DataHolder() { Image1 = image },
							  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue361Tests-DataHolder xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Name>name</Name><Image1 /><Index>13</Index></Issue361Tests-DataHolder>");
		}

		[Fact]
		void VerifyCustomSerialization_CircularReferenceOfRoot_DoesThrow()
		{
			var serializer = new ConfigurationContainer().EnableParameterizedContentWithPropertyAssignments()
														 .Type<AdornedImage>()
														 .Register().Serializer().Using(new AdornedImageSerializer())
														 .Create()
														 .ForTesting();

			var image = new AdornedImage();

			Assert.Throws<CircularReferencesDetectedException >(() => serializer.Serialize(new DataHolder() { Image1 = image, Image2 = image }));
		}

		class AdornedImageSerializer : ISerializer<AdornedImage>
		{
			public AdornedImage Get(IFormatReader parameter)
			{
				return new AdornedImage();
			}

			public void Write(IFormatWriter writer, AdornedImage instance)
			{

			}
		}

		class AdornedImage
		{
			public vector Vector { [UsedImplicitly] get; set; }

			public AdornedImage()
			{
				var data = new length(23);
				Vector = new vector(data, data);
			}
		}

		class DataHolder
		{
			public string Name { get; set; }
			public AdornedImage Image1 { get; set; }
			public AdornedImage Image2 { get; set; }
			public int Index { get; set; }

			public DataHolder()
			{
				Name = "name";
				Index = 13;
			}
		}

		class length
		{
			public length(int value)
			{
				Value = value;
			}

			public int Value { [UsedImplicitly] get; }
		}

		struct vector
		{
			public vector(length l1, length l2)
			{
				L1 = l1;
				L2 = l2;
			}

			public length L1 { [UsedImplicitly] get; }
			public length L2 { [UsedImplicitly] get; }
		}
	}
}