using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Alterations {
	sealed class ValuePipelineSerializers<T> : FixedInstanceSource<IContentSerializer<T>>
	{
		public ValuePipelineSerializers(IContentSerializer<T> content, IPipeline<T> pipeline)
			: base(new ContentSerializer<T>(content, new PipelineWriter<T>(content, pipeline))) {}
	}
}