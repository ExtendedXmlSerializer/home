namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class Enhancer : IEnhancer
	{
		readonly IMarkupExtensions _container;

		public Enhancer(IMarkupExtensions container)
		{
			_container = container;
		}

		public ContentModel.ISerializer Get(ContentModel.ISerializer parameter) => new MarkupExtensionAwareSerializer(_container, parameter);
	}
}