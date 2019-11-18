namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	/// <summary>
	/// Represents a markup extension, modeled after System.Xaml.
	/// </summary>
	public interface IMarkupExtension
	{
		/// <summary>
		/// Given the service provided, retrieves a value.
		/// </summary>
		/// <param name="serviceProvider">The service provider for any necessary service location.</param>
		/// <returns>The resulting value of the request.</returns>
		object ProvideValue(System.IServiceProvider serviceProvider);
	}
}