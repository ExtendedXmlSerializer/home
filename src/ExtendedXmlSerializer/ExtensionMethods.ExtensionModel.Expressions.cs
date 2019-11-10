using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Coercion;
using ExtendedXmlSerializer.ExtensionModel.Expressions;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		public static IConfigurationContainer EnableExpressions(this IConfigurationContainer @this)
		{
			@this.Root
			     .Apply<CoercionExtension>()
			     .Extend(ExpressionsExtension.Default);
			return @this;
		}
	}
}
