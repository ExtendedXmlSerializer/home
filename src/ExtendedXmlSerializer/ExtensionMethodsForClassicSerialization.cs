using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ExtensionModel.Xml.Classic;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// Extension methods that assist or enable functionality found within the extension model namespace for classic
	/// serialization (<see cref="ExtensionModel.Xml.Classic"/>).
	/// </summary>
	public static class ExtensionMethodsForClassicSerialization
	{
		/// <summary>
		/// Used to create a <see cref="XmlParserContext"/> from an XmlNameTable.
		/// </summary>
		/// <param name="this">The XmlNameTable from which to create the context.</param>
		/// <returns>The context.</returns>
		public static XmlParserContext Context(this XmlNameTable @this)
			=> XmlParserContexts.Default.Get(@this ?? new NameTable());

		/// <summary>
		/// Convenience method to retrieve a member element from a provided <see cref="XElement"/>.
		/// </summary>
		/// <param name="this">The provided XElement to query.</param>
		/// <param name="name">The member name used to query the provided XElement.</param>
		/// <returns>The located XElement</returns>
		public static XElement Member(this XElement @this, string name)
			=> @this.Element(XName.Get(name, @this.Name.NamespaceName));

		/// <summary>
		/// Applies the classic emit behavior (<see cref="EmitBehaviors.Classic"/>) and processes lists and dictionaries
		/// without members.  That is, if your list or dictionary is extended with its own properties and members, they will
		/// not be serialized nor deserialized if applied in the document.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer EnableClassicMode(this IConfigurationContainer @this)
			=> @this.Emit(EmitBehaviors.Classic).Extend(ClassicExtension.Default);

		/// <summary>
		/// Enables the xsi:type for classic deserialization purposes.  This will be used to resolve types during the
		/// serialization process if no other type resolution mechanisms are successful in resolving a type.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/261"/>
		public static IConfigurationContainer EnableClassicSchemaTyping(this IConfigurationContainer @this)
			=> @this.Extend(SchemaTypeExtension.Default);

		/// <summary>
		/// Enables `ArrayOfT` and `ListOfT` naming conventions for arrays and lists, respectively.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer EnableClassicListNaming(this IConfigurationContainer @this)
			=> @this.Extend(ClassicListNamingExtension.Default);

		/// <summary>
		/// Used to inspect a type for an <see cref="XmlRootAttribute"/> and if not found, a <see cref="XmlTypeAttribute"/> to
		/// establish its identity.  If both are found, the <see cref="XmlRootAttribute"/> takes precedence, but if any values
		/// there are empty or null, the <see cref="XmlTypeAttribute"/> values will be used, instead.  Using such an approach,
		/// you can use an URI -- either <see cref="XmlRootAttribute.Namespace"/> or <see cref="XmlTypeAttribute.Namespace"/>
		/// -- to specify the type's namespace and the entity's name by using <see cref="XmlRootAttribute.ElementName"/> or
		/// <see cref="XmlTypeAttribute.TypeName"/>) for the that value.  If no name value is found on either, the type's name
		/// will be used instead.
		/// </summary>
		/// <typeparam name="T">The subject type to inspect.</typeparam>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/175" />
		public static IConfigurationContainer InspectingType<T>(this IConfigurationContainer @this)
			=> @this.InspectingTypes(Support<T>.Key.Yield());

		/// <summary>
		/// Inspects a collection of types to inspect for their identities.  This is done by querying the existence of an
		/// <see cref="XmlRootAttribute"/> on each type, and if not found, a <see cref="XmlTypeAttribute" /> to establish its
		/// identity.  If both are found, the <see cref="XmlRootAttribute"/> takes precedence, but if any values there are
		/// empty or null, the <see cref="XmlTypeAttribute" /> values will be used, instead.  Using such an approach, you can
		/// use an URI -- either <see cref="XmlRootAttribute.Namespace"/> or <see cref="XmlTypeAttribute.Namespace" /> -- to
		/// specify the type's namespace and the entity's name by using <see cref="XmlRootAttribute.ElementName" /> or
		/// <see cref="XmlTypeAttribute.TypeName"/>) for the that value.  If no name value is found on either, the type's name
		/// will be used instead.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="types">The list of types to inspect.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/175"/>
		public static IConfigurationContainer InspectingTypes(this IConfigurationContainer @this,
		                                                      IEnumerable<Type> types)
			=> @this.Extend(new ClassicIdentificationExtension(types.YieldMetadata().ToList()));

		/// <summary>
		/// Adds basic support for the <see cref="XmlTextAttribute" />.  Note that this is not a very robust solution and
		/// there are deficiencies in the fidelity with the classic serializer's implementation.  Use with care, and make a
		/// wish. 😆
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/192"/>
		public static IConfigurationContainer EnableXmlText(this IConfigurationContainer @this)
			=> @this.Extend(XmlTextExtension.Default);
	}
}