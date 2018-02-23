using System.IO;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content.Instances;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Alterations {
	sealed class ContentPipelineFormatWriters : ConditionalSource<Stream, IFormatWriter>, IFormatWriters
	{
		public ContentPipelineFormatWriters(
			Configuration.MetadataValue<WriteContentPipelineProperty, IPipeline<string>> property,
			IFormatWriters writers)
			: this(property.In(RootInstanceTypes.Default), writers) {}

		public ContentPipelineFormatWriters(ISpecificationSource<Stream, IPipeline<string>> types,
		                                    IFormatWriters writers)
			: base(types, new FormatWriters(types, writers), writers) {}
	}
}