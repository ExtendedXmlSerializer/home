namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	public interface IMarkupExtension
	{
		object ProvideValue(System.IServiceProvider serviceProvider);
	}
}