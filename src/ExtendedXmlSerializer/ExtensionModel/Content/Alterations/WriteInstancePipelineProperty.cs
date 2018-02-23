namespace ExtendedXmlSerializer.ExtensionModel.Content.Alterations {
	[Extension(typeof(ValueAlterationExtension))]
	public sealed class WriteInstancePipelineProperty<T> : MetadataProperty<IPipeline<T>>
	{
		public static WriteInstancePipelineProperty<T> Default { get; } = new WriteInstancePipelineProperty<T>();
		WriteInstancePipelineProperty() : base(_ => new Pipeline<T>()) {}
	}
}