using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel
{
	/// <summary>
	/// Intended to be a container of commonly-used or shared values used throughout the containing namespace of this class.
	/// </summary>
	public static class Defaults
	{
		/// <summary>
		/// Convenience property used to store the root object as a framework type.  Used for namespace resolution.
		/// </summary>
		public static TypeInfo FrameworkType { get; } = typeof(IExtendedXmlSerializer).GetTypeInfo();

		/// <summary>
		/// Identifier used to identify framework components used in this assembly.
		/// </summary>
		public static string Identifier { get; } = "https://extendedxmlserializer.github.io/v2";
	}
}