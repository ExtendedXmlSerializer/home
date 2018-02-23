using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Alterations
{
	sealed class PipelineWriter<T> : IContentWriter<T>
	{
		readonly IContentWriter<T> _writer;
		readonly IPipeline<T>      _pipeline;

		public PipelineWriter(IContentWriter<T> writer, IPipeline<T> pipeline)
		{
			_writer   = writer;
			_pipeline = pipeline;
		}

		public void Execute(Writing<T> parameter)
		{
			_writer.Execute(new Writing<T>(parameter.Writer, _pipeline.Alter(parameter.Instance)));
		}
	}
}