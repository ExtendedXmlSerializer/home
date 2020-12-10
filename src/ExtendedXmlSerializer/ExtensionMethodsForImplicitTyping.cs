using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// A set of extension methods that assist in enabling implicit typing for the configuration container.
	/// </summary>
	public static class ExtensionMethodsForImplicitTyping
	{
		/// <summary>
		/// Enables implicit typing on a configuration container, which will create a serializer that does not emit xmlns
		/// namespaces for the provided types, nor will it require them during deserialization.  This makes XML more JSON-like
		/// in its resulting output and required input, leading to more streamlined and less verbose documents.  Note that
		/// this feature has limits, namely that there can be only one type with any given name.  The type name is considered
		/// the unique identifier and if there is more than one type with the same name an exception is thrown.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="types">The types to register.  Ensure the provided types all have unique names or else an exception
		/// is thrown.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer EnableImplicitTyping(this IConfigurationContainer @this,
		                                                           params Type[] types)
			=> @this.Extend(new ImplicitTypingExtension(types.ToMetadata()));

		/// <summary>
		/// Convenience method to pass in a source type enumerable and resolve it into an array to pass to
		/// <see cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])" />.  This is primarily used by the
		/// EnableImplicitTyping* methods, but can also be used with any enumerable of <see cref="System.Type"/>.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="types">The types to register as implicit.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])"/>
		public static IConfigurationContainer EnableImplicitTyping(this IConfigurationContainer @this,
		                                                           IEnumerable<Type> types)
			=> @this.EnableImplicitTyping(types.ToArray());

		/// <summary>
		/// Convenience method to enable implicit typing on a container, using all public-nested types found within the
		/// provided subject type.  All public nested types found within the provided subject type will be included as an
		/// implicit type.
		/// </summary>
		/// <typeparam name="T">The subject type to query for type resolution.</typeparam>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])"/>
		public static IConfigurationContainer EnableImplicitTypingFromPublicNested<T>(
			this IConfigurationContainer @this)
			=> @this.EnableImplicitTyping(new PublicNestedTypes<T>());

		/// <summary>
		/// Convenience method to enable implicit typing on a container, using all public-nested types found within the
		/// provided subject type.  All public nested types found within the provided subject type will be included as an
		/// implicit type.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="type">The subject type to query for type resolution.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])"/>
		public static IConfigurationContainer EnableImplicitTypingFromPublicNested(
			this IConfigurationContainer @this, Type type)
			=> @this.EnableImplicitTyping(new PublicNestedTypes(type));

		/// <summary>
		/// Convenience method to enable implicit typing on a container, using all nested types -- private or otherwise --
		/// found within the provided subject type.  All nested types found within the provided subject type will be
		/// included and registered as an implicit type.
		/// </summary>
		/// <typeparam name="T">The subject type to query for type resolution.</typeparam>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])"/>
		public static IConfigurationContainer EnableImplicitTypingFromNested<T>(this IConfigurationContainer @this)
			=> @this.EnableImplicitTyping(new NestedTypes<T>());

		/// <summary>
		/// Convenience method to enable implicit typing on a container, using all nested types -- private or otherwise --
		/// found within the provided subject type.  All nested types found within the provided subject type will be
		/// included and registered as an implicit type.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="type">The subject type to query for type resolution.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])"/>
		public static IConfigurationContainer EnableImplicitTypingFromNested(this IConfigurationContainer @this, Type type)
			=> @this.EnableImplicitTyping(new NestedTypes(type));


		/// <summary>
		/// Convenience method to enable implicit typing on a container, using all found types within the provided subject
		/// type's assembly.  All types found within the provided subject type's assembly will be included and registered as
		/// an implicit type.  Use this with care and ensure that the names of all the types found within the assembly are
		/// unique. Otherwise, an exception will be thrown if more than one type share the same name.
		/// </summary>
		/// <typeparam name="T">The subject type to query for type resolution.</typeparam>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])"/>
		public static IConfigurationContainer EnableImplicitTypingFromAll<T>(this IConfigurationContainer @this)
			=> @this.EnableImplicitTyping(new AllAssemblyTypes<T>());

		/// <summary>
		/// Convenience method to enable implicit typing on a container, using all found types within the provided subject
		/// type's assembly.  All types found within the provided subject type's assembly will be included and registered as
		/// an implicit type.  Use this with care and ensure that the names of all the types found within the assembly are
		/// unique. Otherwise, an exception will be thrown if more than one type share the same name.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="type">The subject type to query for type resolution.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])"/>
		public static IConfigurationContainer EnableImplicitTypingFromAll(this IConfigurationContainer @this, Type type)
			=> @this.EnableImplicitTyping(new AllAssemblyTypes(type));

		/// <summary>
		/// Convenience method to enable implicit typing on a container, using all found public types within the provided
		/// subject type's assembly.  All public types found within the provided subject type's assembly will be included and
		/// registered as an implicit type.  Use this with care and ensure that the names of all the public types found within
		/// the assembly are unique.  Otherwise, an exception will be thrown if more than one type share the same name.
		/// </summary>
		/// <typeparam name="T">The subject type to query for type resolution.</typeparam>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])"/>
		public static IConfigurationContainer EnableImplicitTypingFromPublic<T>(this IConfigurationContainer @this)
			=> @this.EnableImplicitTyping(new PublicAssemblyTypes<T>());

		/// <summary>
		/// Convenience method to enable implicit typing on a container, using all found public types within the provided
		/// subject type's assembly.  All public types found within the provided subject type's assembly will be included and
		/// registered as an implicit type.  Use this with care and ensure that the names of all the public types found within
		/// the assembly are unique.  Otherwise, an exception will be thrown if more than one type share the same name.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="type">The subject type to query for type resolution.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])"/>
		public static IConfigurationContainer EnableImplicitTypingFromPublic(this IConfigurationContainer @this, Type type)
			=> @this.EnableImplicitTyping(new PublicAssemblyTypes(type));

		/// <summary>
		/// Convenience method to enable implicit typing on a container, using all found types within the provided subject
		/// type's namespace.  All types found within the provided subject type's namespace will be included and registered as
		/// an implicit type.  Use this with care and ensure that the names of all the types found within the namespace are
		/// unique. Otherwise, an exception will be thrown if more than one type share the same name.
		/// </summary>
		/// <typeparam name="T">The subject type to query for type resolution.</typeparam>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])"/>
		public static IConfigurationContainer EnableImplicitTypingFromNamespace<T>(this IConfigurationContainer @this)
			=> @this.EnableImplicitTyping(new AllTypesInSameNamespace<T>());

		/// <summary>
		/// Convenience method to enable implicit typing on a container, using all found types within the provided subject
		/// type's namespace.  All types found within the provided subject type's namespace will be included and registered as
		/// an implicit type.  Use this with care and ensure that the names of all the types found within the namespace are
		/// unique. Otherwise, an exception will be thrown if more than one type share the same name.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="type">The subject type to query for type resolution.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])"/>
		public static IConfigurationContainer EnableImplicitTypingFromNamespace(this IConfigurationContainer @this, Type type)
			=> @this.EnableImplicitTyping(new AllTypesInSameNamespace(type));

		/// <summary>
		/// Convenience method to enable implicit typing on a container, using all found public types within the provided
		/// subject type's namespace.  All public types found within the provided subject type's namespace will be included and
		/// registered as an implicit type.  Use this with care and ensure that the names of all the public types found within
		/// the namespace are unique.  Otherwise, an exception will be thrown if more than one type share the same name.
		/// </summary>
		/// <typeparam name="T">The subject type to query for type resolution.</typeparam>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])"/>
		public static IConfigurationContainer EnableImplicitTypingFromNamespacePublic<T>(
			this IConfigurationContainer @this)
			=> @this.EnableImplicitTyping(new PublicTypesInSameNamespace<T>());

		/// <summary>
		/// Convenience method to enable implicit typing on a container, using all found public types within the provided
		/// subject type's namespace.  All public types found within the provided subject type's namespace will be included and
		/// registered as an implicit type.  Use this with care and ensure that the names of all the public types found within
		/// the namespace are unique.  Otherwise, an exception will be thrown if more than one type share the same name.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="type">The subject type to query for type resolution.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso cref="EnableImplicitTyping(IConfigurationContainer,System.Type[])"/>
		public static IConfigurationContainer EnableImplicitTypingFromNamespacePublic(
			this IConfigurationContainer @this, Type type)
			=> @this.EnableImplicitTyping(new PublicTypesInSameNamespace(type));
	}
}
