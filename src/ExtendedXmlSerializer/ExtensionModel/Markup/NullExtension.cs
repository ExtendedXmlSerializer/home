using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	/// <summary>
	/// Markup extension to return a `null` value.
	/// </summary>
	[UsedImplicitly]
	public sealed class NullExtension : IMarkupExtension
	{
		/// <inheritdoc />
		public object ProvideValue(System.IServiceProvider serviceProvider) => null;
	}
}