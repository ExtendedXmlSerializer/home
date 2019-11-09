using System.Reflection;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.ContentModel
{
	public static class Defaults
	{
		public static TypeInfo FrameworkType { get; } = typeof(IExtendedXmlSerializer).GetTypeInfo();

		public static string Identifier { get; } = "https://extendedxmlserializer.github.io/v2";
	}
}