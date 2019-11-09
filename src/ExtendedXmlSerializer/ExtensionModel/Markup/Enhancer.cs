using ExtendedXmlSerializer.ContentModel;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class Enhancer : IEnhancer
	{
		readonly IMarkupExtensions _container;

		public Enhancer(IMarkupExtensions container)
		{
			_container = container;
		}

		public ISerializer Get(ISerializer parameter) => new MarkupExtensionAwareSerializer(_container, parameter);
	}
}