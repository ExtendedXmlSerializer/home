using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Markup;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		public static IConfigurationContainer EnableMarkupExtensions(this IConfigurationContainer @this)
			=> @this.EnableExpressions()
			        .Alter(MarkupExtensionConverterAlteration.Default)
			        .Extend(MarkupExtension.Default);
	}
}
