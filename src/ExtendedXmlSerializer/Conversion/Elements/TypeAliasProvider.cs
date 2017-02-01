using System.Reflection;
using System.Xml.Serialization;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	class TypeAliasProvider : AliasProviderBase<TypeInfo>
	{
		public static TypeAliasProvider Default { get; } = new TypeAliasProvider();
		TypeAliasProvider() {}

		protected override string GetItem(TypeInfo parameter) =>
			parameter.GetCustomAttribute<XmlRootAttribute>()?.ElementName;
	}
}