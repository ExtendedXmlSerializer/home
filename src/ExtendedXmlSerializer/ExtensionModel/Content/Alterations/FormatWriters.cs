using System.IO;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Alterations {
	sealed class FormatWriters : IFormatWriters
	{
		readonly IParameterizedSource<Stream, IPipeline<string>> _pipeline;
		readonly IFormatWriters                                  _writers;

		public FormatWriters(IParameterizedSource<Stream, IPipeline<string>> pipeline, IFormatWriters writers)
		{
			_pipeline = pipeline;
			_writers  = writers;
		}

		public IFormatWriter Get(Stream parameter) => new ContentPipelineFormatWriter(_writers.Get(parameter), _pipeline.Get(parameter));
	}
}