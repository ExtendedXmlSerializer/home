using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	[UsedImplicitly]
	public sealed class NullExtension : IMarkupExtension
	{
		public object ProvideValue(System.IServiceProvider serviceProvider) => null;
	}
}