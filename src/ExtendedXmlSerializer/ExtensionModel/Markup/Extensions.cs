using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Expressions;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	public static class Extensions
	{
		public static IConfigurationContainer EnableMarkupExtensions(this IConfigurationContainer @this)
			=> @this.EnableExpressions()
			        .Alter(MarkupExtensionConverterAlteration.Default)
			        .Extend(MarkupExtension.Default);
	}
}