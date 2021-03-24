using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Coercion;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.Expressions;
using ExtendedXmlSerializer.ExtensionModel.Instances;
using ExtendedXmlSerializer.ExtensionModel.Markup;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

// ReSharper disable TooManyArguments

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// Extension methods that assist or enable functionality found within the extension model namespace (
	/// <see cref="ExtensionModel"/>).
	/// </summary>
	public static class ExtensionMethodsForExtensionModel
	{
		/// <summary>
		/// Enables the use of target instances to read values into when serialization occurs.  Use this in conjunction with
		/// <see cref="UsingTarget{T}" /> to establish a target instance during deserialization.  In this context, values read
		/// from the source document will be assigned to the provided existing target instance rather than the default
		/// behavior of creating a new instance altogether.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer AllowTargetInstances(this IConfigurationContainer @this)
			=> @this.Root.With<ExistingInstanceExtension>().Return(@this);

		/// <summary>
		/// Specifies an existing reference to use as target for deserialization of values.  Read values during
		/// deserialization will be read and assigned into the provided target instance. NOTICE: Be sure to call
		/// <see cref="AllowTargetInstances"/> when configuring the container before using this method.
		/// </summary>
		/// <typeparam name="T">The instance type.</typeparam>
		/// <param name="this">The serializer</param>
		/// <param name="instance">The instance to deserialize</param>
		/// <returns>A deserialization context that will assign values to the provided instance.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/230" />
		public static ReferencedDeserializationContext<T> UsingTarget<T>(this IExtendedXmlSerializer @this, T instance)
			where T : class
			=> new ReferencedDeserializationContext<T>(@this, instance);

		/// <summary>
		/// Enables thread protection and wraps a simple `lock` around the reading and writing of the created serializer.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer EnableThreadProtection(this IConfigurationContainer @this)
			=> @this.Extend(ThreadProtectionExtension.Default);

		/// <summary>
		/// Enables member exception handling during serialization and deserialization.  By default when errors are
		/// encountered during these processes the exception is simply thrown without much context or detail.  This is for
		/// performance considerations and to cut down on try/catches.  Enabling this feature wraps
		/// serialization/deserialization in try-catches to provide more detail when exceptions occur.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso href="https://mattwarren.org/2016/12/20/Why-Exceptions-should-be-Exceptional/"/>
		public static IConfigurationContainer EnableMemberExceptionHandling(this IConfigurationContainer @this)
			=> @this.Extend(MemberExceptionHandlingExtension.Default);

		/// <summary>
		/// Creates a new context for Unknown Content and allows the user to determine how the serializer behaves when it
		/// encounters unknown content during deserialization.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The UnknownContentContext for further action and configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/271#issuecomment-550976753" />
		public static UnknownContentContext WithUnknownContent(this IConfigurationContainer @this)
			=> new UnknownContentContext(@this);

		/// <summary>
		/// This is considered internal framework functionality and is not intended to be used from your code.  However. 😁
		/// This enables root instances on a serializer.  When used, components that use the <see cref="IRootInstances"/>
		/// interface will have access to the root instance that was passed in for serialization, usually by using
		/// <see cref="IExtendedXmlSerializer"/>'s `Serialize` method.
		/// </summary>
		/// <typeparam name="T">The root context type.</typeparam>
		/// <param name="this">The root context (usually an <see cref="IConfigurationContainer"/>) to configure.</param>
		/// <returns>The configured root context (usually an <see cref="IConfigurationContainer"/>).</returns>
		public static T EnableRootInstances<T>(this T @this) where T : IRootContext
			=> @this.Root.With<RootInstanceExtension>().Return(@this);

		/// <summary>
		/// Do not use this method unless you are familiar with its behavior.  This removes static reference checking which
		/// speeds up performance but will lead to endless recursion if your graph indeed has a circular reference within it.
		///
		/// Note that this is very incompatible with EnableReferences. Please see provided GitHub link for more context and
		/// information.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="enabled">Specifies if static-reference checking should be enabled.  The default behavior is true.
		/// Specifying false will ignore static reference checking and improve performance for graphs that may have circular
		/// references (but shouldn't).</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/511" />
		public static IConfigurationContainer EnableStaticReferenceChecking(
			this IConfigurationContainer @this, bool enabled)
			=> enabled ? @this : @this.Root.Apply<StaticReferenceCheckingExtension>().Return(@this);

		/// <summary>
		/// This is considered internal framework functionality and is not intended to be used from your code.  However. 😁
		/// This enables the use of expressions within deserialized properties (attached properties or markup extensions), so
		/// that they may be evaluated to a runtime value.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer EnableExpressions(this IConfigurationContainer @this)
			=> @this.Root.Apply<CoercionExtension>()
			        .Extend(ExpressionsExtension.Default)
			        .Return(@this);

		/// <summary>
		/// Enables markup extensions support for the container.  This allows you to create markup extensions and enable them
		/// within your XML, much like Xaml does for WPF.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso href="https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/markup-extensions-and-wpf-xaml"/>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/wiki/Features#xaml-ness-markup-extensions"/>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/blob/25514a768f7dc6b3166119254a1bd80ea13e1dbe/test/ExtendedXmlSerializer.Tests/ExtensionModel/Markup/MarkupExtensionTests.cs"/>
		public static IConfigurationContainer EnableMarkupExtensions(this IConfigurationContainer @this)
			=> @this.EnableExpressions()
			        .Alter(MarkupExtensionConverterAlteration.Default)
			        .Extend(MarkupExtension.Default);

		/// <summary>
		/// Enables all constructors -- in particular, private ones -- as candidates for selection when selecting a
		/// constructor to activate during deserialization.  By default, only public constructors are considered.  Calling
		/// this method configures the serializer so that all constructors -- private and otherwise -- are also considered.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer EnableAllConstructors(this IConfigurationContainer @this)
			=> @this.Extend(AllConstructorsExtension.Default);

		/// <summary>
		/// Enables the use of types found in the `System.Collections.Immutable` namespace.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/485" />
		public static IConfigurationContainer EnableImmutableTypes(this IConfigurationContainer @this)
			=> @this.Extend(ImmutableListExtension.Default)
			        .Extend(ImmutableHashSetExtension.Default)
			        .Extend(ImmutableSortedSetExtension.Default)
			        .Extend(ImmutableDictionariesExtension.Default)
			        .Extend(ImmutableSortedDictionariesExtension.Default);

		#region Obsolete

		/// <summary>
		/// This is an unused method and will be removed in a future version.
		/// </summary>
		/// <param name="this"></param>
		/// <returns></returns>
		[Obsolete("This method is unused and will be removed in a future version.")]
		public static IEnumerable<Type> WithArrayTypes(this IEnumerable<Type> @this)
			=> @this.ToArray()
			        .Alter(x => x.Concat(x.Select(y => y.MakeArrayType()))
			                     .ToArray());

		/// <summary>
		/// Use `IExtendedXmlSerializer.UsingTarget.Deserialize` instead.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this"></param>
		/// <param name="existing"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		/// <seealso cref="ReferencedDeserializationContext{T}.Deserialize(string)"/>
		[Obsolete("Use IExtendedXmlSerializer.UsingTarget.Deserialize instead.")]
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, T existing, string data) where T : class
			=> @this.UsingTarget(existing)
			        .Deserialize(data);

		/// <summary>
		/// Use `IExtendedXmlSerializer.UsingTarget.Deserialize` instead.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this"></param>
		/// <param name="existing"></param>
		/// <param name="settings"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		/// <seealso cref="ReferencedDeserializationContext{T}.Deserialize(XmlReaderSettings,string)"/>
		[Obsolete("Use IExtendedXmlSerializer.UsingTarget.Deserialize instead.")]
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, T existing, XmlReaderSettings settings,
		                               string data) where T : class
			=> @this.UsingTarget(existing)
			        .Deserialize(settings, data);

		/// <summary>
		/// Use `IExtendedXmlSerializer.UsingTarget.Deserialize` instead.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this"></param>
		/// <param name="existing"></param>
		/// <param name="stream"></param>
		/// <returns></returns>
		/// <seealso cref="ReferencedDeserializationContext{T}.Deserialize(Stream)"/>
		[Obsolete("Use IExtendedXmlSerializer.UsingTarget.Deserialize instead.")]
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, T existing, Stream stream) where T : class
			=> @this.UsingTarget(existing).Deserialize(stream);

		/// <summary>
		/// Use `IExtendedXmlSerializer.UsingTarget.Deserialize` instead.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this"></param>
		/// <param name="existing"></param>
		/// <param name="settings"></param>
		/// <param name="stream"></param>
		/// <returns></returns>
		/// <seealso cref="ReferencedDeserializationContext{T}.Deserialize(XmlReaderSettings,Stream)"/>
		[Obsolete("Use IExtendedXmlSerializer.UsingTarget.Deserialize instead.")]
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, T existing, XmlReaderSettings settings,
		                               Stream stream)
			where T : class
			=> @this.UsingTarget(existing).Deserialize(settings, stream);

		/// <summary>
		/// Deprecated.  Use `AllowTargetInstances` instead.
		/// </summary>
		/// <param name="this"></param>
		/// <returns></returns>
		/// <seealso cref="AllowTargetInstances" />
		[Obsolete("Use AllowTargetInstances instead.")]
		public static IConfigurationContainer AllowExistingInstances(this IConfigurationContainer @this)
			=> @this.AllowTargetInstances();

		/// <summary>
		/// This method is being deprecated.  Please use `ConfigurationContainer.WithUnknownContent.Call` instead.
		/// </summary>
		/// <param name="this"></param>
		/// <param name="onMissing"></param>
		/// <returns></returns>
		/// <seealso cref="UnknownContentContext.Call"/>
		[Obsolete(
			         "This method is being deprecated.  Please use ConfigurationContainer.WithUnknownContent.Call instead.")]
		public static IConfigurationContainer EnableUnknownContentHandling(this IConfigurationContainer @this,
		                                                                   Action<IFormatReader> onMissing)
			=> @this.WithUnknownContent().Call(onMissing);

		#endregion
	}
}