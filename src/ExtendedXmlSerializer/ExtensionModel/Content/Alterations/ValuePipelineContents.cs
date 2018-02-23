using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Alterations {
	sealed class ValuePipelineContents<T> : ConditionalContents<T>, IContents<T>
	{
		public ValuePipelineContents(Configuration.MetadataValue<WriteInstancePipelineProperty<T>, IPipeline<T>> property,
		                             IContents<T> contents)
			: base(property, new ValuePipelineSerializers<T>(contents.Get(), property.Get(Support<T>.Key)), contents) {}
	}
}