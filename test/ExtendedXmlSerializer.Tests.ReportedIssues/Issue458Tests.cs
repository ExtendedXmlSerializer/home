using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue458Tests
	{
		[Fact]
		public void Verify()
		{
			var instance = new DecoratedImage(null);

			var serializer = new ConfigurationContainer().EnableParameterizedContentWithPropertyAssignments()
			                                             .UseOptimizedNamespaces()
														 //
			                                             .Type<DecoratedImage>()
			                                             .Register()
			                                             .Serializer()
			                                             .Of<ImageSerializer>()
														 //
			                                             .Create()
			                                             .ForTesting();

			serializer.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue458Tests-DecoratedImage xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues"" />");
		}

		public class ImageSerializer : ISerializer<DecoratedImage>
		{
			public DecoratedImage Get(IFormatReader parameter) => new DecoratedImage(null);

			public void Write(IFormatWriter writer, DecoratedImage instance) {}
		}

		public class DecoratedImage
		{
			public DecoratedImage(Metadata metadata) => Metadata = metadata;

			public Metadata Metadata
			{
				// ReSharper disable once ThrowExceptionInUnexpectedLocation
				get => throw new NotImplementedException();
				// ReSharper disable once RedundantAssignment
				set => value = null;
			}
		}

		public class Metadata {}
	}
}