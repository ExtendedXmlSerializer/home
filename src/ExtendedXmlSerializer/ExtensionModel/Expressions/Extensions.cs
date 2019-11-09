using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Coercion;

namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	public static class Extensions
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