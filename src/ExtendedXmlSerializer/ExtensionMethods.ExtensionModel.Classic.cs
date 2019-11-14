using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ExtensionModel.Xml.Classic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		public static IConfigurationContainer EnableClassicMode(this IConfigurationContainer @this)
			=> @this.Emit(EmitBehaviors.Classic)
			        .Extend(ClassicExtension.Default);

		/// <summary>
		/// Enables the xsi:type for legacy deserialization purposes.
		/// https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/261
		/// </summary>
		/// <param name="this"></param>
		/// <returns></returns>
		public static IConfigurationContainer EnableClassicSchemaTyping(this IConfigurationContainer @this)
			=> @this.Extend(SchemaTypeExtension.Default);

		/// <summary>
		/// Enables `ArrayOfT` and `ListOfT` naming conventions for arrays and lists, respectively.
		/// </summary>
		/// <param name="this"></param>
		/// <returns></returns>
		public static IConfigurationContainer EnableClassicListNaming(this IConfigurationContainer @this)
			=> @this.Extend(ClassicListNamingExtension.Default);

		public static IConfigurationContainer InspectingType<T>(this IConfigurationContainer @this)
			=> @this.InspectingTypes(typeof(T).Yield());

		public static IConfigurationContainer InspectingTypes(this IConfigurationContainer @this,
		                                                      IEnumerable<Type> types)
			=> @this.Extend(new ClassicIdentificationExtension(types.YieldMetadata()
			                                                        .ToList()));
		public static IConfigurationContainer EnableXmlText(this IConfigurationContainer @this)
			=> @this.Extend(XmlTextExtension.Default);
	}
}
