namespace ExtendedXmlSerializer.ExtensionModel.Content.Alterations {
	[Extension(typeof(ValueAlterationExtension))]
	sealed class WriteContentPipelineProperty : MetadataProperty<IPipeline<string>>
	{
		public static WriteContentPipelineProperty Default { get; } = new WriteContentPipelineProperty();
		WriteContentPipelineProperty() : base(_ => new Pipeline<string>()) {}
	}
}