using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		public static IConfigurationContainer EnableImplicitTyping(this IConfigurationContainer @this,
		                                                           params Type[] types)
			=> @this.EnableImplicitTyping(types.AsEnumerable());

		public static IConfigurationContainer EnableImplicitTypingFromPublicNested<T>(
			this IConfigurationContainer @this)
			=> @this.EnableImplicitTyping(new PublicNestedTypes<T>());

		public static IConfigurationContainer EnableImplicitTypingFromNested<T>(this IConfigurationContainer @this)
			=> @this.EnableImplicitTyping(new NestedTypes<T>());

		public static IConfigurationContainer EnableImplicitTypingFromAll<T>(this IConfigurationContainer @this)
			=> @this.EnableImplicitTyping(new AllAssemblyTypes<T>());

		public static IConfigurationContainer EnableImplicitTypingFromPublic<T>(this IConfigurationContainer @this)
			=> @this.EnableImplicitTyping(new PublicAssemblyTypes<T>());

		public static IConfigurationContainer EnableImplicitTypingFromNamespace<T>(this IConfigurationContainer @this)
			=> @this.EnableImplicitTyping(new AllTypesInSameNamespace<T>());

		public static IConfigurationContainer EnableImplicitTypingFromNamespacePublic<T>(
			this IConfigurationContainer @this)
			=> @this.EnableImplicitTyping(new PublicTypesInSameNamespace<T>());

		public static IConfigurationContainer EnableImplicitTyping(this IConfigurationContainer @this,
		                                                           IEnumerable<Type> types)
			=> @this.Extend(new ImplicitTypingExtension(types.ToMetadata()));
	}
}
