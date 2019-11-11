using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Coercion;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.Expressions;
using ExtendedXmlSerializer.ExtensionModel.Instances;
using ExtendedXmlSerializer.ExtensionModel.Markup;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

// ReSharper disable TooManyArguments

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	/// <exclude />
	public static partial class ExtensionMethods
	{
		/// <summary>
		/// Enables the use of target instances to read values into when
		/// deserialization occurs.  <seealso cref="UsingTarget{T}" />
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer AllowTargetInstances(this IConfigurationContainer @this)
			=> @this.Root.With<ExistingInstanceExtension>()
			        .Return(@this);

		/// <summary>
		///   <para>
		/// Specifies an existing reference to use as target for deserialization of values.  Read values during
		/// deserialization will be read and assigned into the provided target instance.
		/// </para>
		///   <note type="important">Be sure to call <see cref="M:ExtendedXmlSerializer.ExtensionMethods.AllowExistingInstances(IConfigurationContainer)"/> when configuring the container before using this method.</note>
		/// </summary>
		/// <typeparam name="T">The instance type.</typeparam>
		/// <param name="this">The serializer</param>
		/// <param name="instance">The instance to deserialize</param>
		/// <returns>A <see cref="ReferencedDeserialization(T)"/> context for deserialization on the referenced instance.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/230" />
		public static ReferencedDeserialization<T> UsingTarget<T>(this IExtendedXmlSerializer @this, T instance)
			where T : class
			=> new ReferencedDeserialization<T>(@this, instance);

		/// <summary>
		///   <para>This is an args.</para>
		/// </summary>
		/// <param name="this"></param>
		public static IConfigurationContainer EnableThreadProtection(this IConfigurationContainer @this)
			=> @this.Extend(ThreadProtectionExtension.Default);

		public static IConfigurationContainer EnableMemberExceptionHandling(this IConfigurationContainer @this)
			=> @this.Extend(MemberExceptionHandlingExtension.Default);

		/// <summary>
		/// Creates a new context for Unknown Content and allows the user to determine how the serializer behaves when it encounters unknown content during deserialization.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>UnknownContentContext.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/271#issuecomment-550976753" />
		public static UnknownContentContext WithUnknownContent(this IConfigurationContainer @this)
			=> new UnknownContentContext(@this);

		public static T EnableRootInstances<T>(this T @this) where T : IRootContext
			=> @this.Root.With<RootInstanceExtension>()
			        .Return(@this);

		public static IConfigurationContainer EnableExpressions(this IConfigurationContainer @this)
			=> @this.Root.Apply<CoercionExtension>()
			        .Extend(ExpressionsExtension.Default)
			        .Return(@this);

		public static IConfigurationContainer EnableMarkupExtensions(this IConfigurationContainer @this)
			=> @this.EnableExpressions()
			        .Alter(MarkupExtensionConverterAlteration.Default)
			        .Extend(MarkupExtension.Default);

		public static IConfigurationContainer EnableAllConstructors(this IConfigurationContainer @this)
			=> @this.Extend(AllConstructorsExtension.Default);

		#region Obsolete

		/// <exclude />
		[Obsolete("This method is unused and will be removed in a future version.")]
		public static IEnumerable<Type> WithArrayTypes(this IEnumerable<Type> @this)
			=> @this.ToArray()
			        .Alter(x => x.Concat(x.Select(y => y.MakeArrayType()))
			                     .ToArray());

		/// <exclude />
		[Obsolete("Use IExtendedXmlSerializer.UsingTarget.Deserialize instead.")]
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, T existing, string data) where T : class
			=> @this.UsingTarget(existing)
			        .Deserialize(data);

		/// <exclude />
		[Obsolete("Use IExtendedXmlSerializer.UsingTarget.Deserialize instead.")]
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, T existing, XmlReaderSettings settings,
		                               string data) where T : class
			=> @this.UsingTarget(existing)
			        .Deserialize(settings, data);

		/// <exclude />
		[Obsolete("Use IExtendedXmlSerializer.UsingTarget.Deserialize instead.")]
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, T existing, Stream stream) where T : class
			=> @this.UsingTarget(existing)
			        .Deserialize(stream);

		/// <exclude />
		[Obsolete("Use IExtendedXmlSerializer.UsingTarget.Deserialize instead.")]
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, T existing, XmlReaderSettings settings,
		                               Stream stream) where T : class
			=> @this.UsingTarget(existing)
			        .Deserialize(settings, stream);

		/// <exclude />
		[Obsolete("Use AllowTargetInstances instead.")]
		public static IConfigurationContainer AllowExistingInstances(this IConfigurationContainer @this)
			=> @this.AllowTargetInstances();

		/// <exclude />
		[Obsolete(
			"This method is being deprecated.  Please use ConfigurationContainer.WithUnknownContent.Call instead.")]
		public static IConfigurationContainer EnableUnknownContentHandling(this IConfigurationContainer @this,
		                                                                   Action<IFormatReader> onMissing)
			=> @this.WithUnknownContent()
			        .Call(onMissing);

		#endregion
	}
}